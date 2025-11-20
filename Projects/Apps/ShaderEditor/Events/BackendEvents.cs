using System.Runtime.CompilerServices;
using System.Text;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D.Compilers;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;

namespace ShaderEditor.Events;

public record GLDisposingEvent;

public abstract record RenderRequest
{
	public int Priority { get; set; } = 0;
}

public record MeshRenderRequest : RenderRequest
{
	public required ComPtr<ID3D11Buffer> IndexBuffer;

	public uint IndexCount;
	public required ComPtr<ID3D11InputLayout> InputLayout;
	public required ComPtr<ID3D11PixelShader> PixelShader;
	public required ComPtr<ID3D11Buffer> VertexBuffer;
	public uint VertexOffset = 0U;
	public required ComPtr<ID3D11VertexShader> VertexShader;
	public uint VertexStride = 3U * sizeof(float);

	public unsafe void Execute(ComPtr<ID3D11DeviceContext> deviceContext)
	{
		deviceContext.IASetPrimitiveTopology(D3DPrimitiveTopology.D3DPrimitiveTopologyTrianglelist);
		deviceContext.IASetInputLayout(InputLayout);
		deviceContext.IASetVertexBuffers(0, 1, VertexBuffer, in VertexStride, in VertexOffset);
		deviceContext.IASetIndexBuffer(IndexBuffer, Format.FormatR32Uint, 0);

		deviceContext.VSSetShader(VertexShader, ref Unsafe.NullRef<ComPtr<ID3D11ClassInstance>>(), 0);
		deviceContext.PSSetShader(PixelShader, ref Unsafe.NullRef<ComPtr<ID3D11ClassInstance>>(), 0);

		deviceContext.DrawIndexed(IndexCount, 0, 0);
	}
}

public record ShaderRenderRequest : RenderRequest
{
	public required Action<ComPtr<ID3D11Device>, ComPtr<ID3D11DeviceContext>> ShaderSetupAction { get; init; }

	public void Execute(ComPtr<ID3D11Device> device, ComPtr<ID3D11DeviceContext> deviceContext)
	{
		ShaderSetupAction(device, deviceContext);
	}
}

public static class RenderRequestFactory
{
	public static MeshRenderRequest CreateMeshRenderRequest(
		ComPtr<ID3D11Device> device,
		float[] vertices,
		uint[] indices,
		string shaderSource,
		string inputSemanticName = "POS")
	{
		// Create vertex buffer
		var vertexBuffer = CreateVertexBuffer(device, vertices);
		var indexBuffer = CreateIndexBuffer(device, indices);

		// Compile shaders
		var (vertexShader, pixelShader, inputLayout) = CompileShaders(device, shaderSource, inputSemanticName);

		return new MeshRenderRequest
		{
			VertexBuffer = vertexBuffer,
			IndexBuffer = indexBuffer,
			InputLayout = inputLayout,
			VertexShader = vertexShader,
			PixelShader = pixelShader,
			IndexCount = (uint)indices.Length
		};
	}

	private static unsafe ComPtr<ID3D11Buffer> CreateVertexBuffer(ComPtr<ID3D11Device> device, float[] vertices)
	{
		var bufferDesc = new BufferDesc
		{
			ByteWidth = (uint)(vertices.Length * sizeof(float)),
			Usage = Usage.Default,
			BindFlags = (uint)BindFlag.VertexBuffer
		};

		fixed (float* vertexData = vertices)
		{
			var subresourceData = new SubresourceData { PSysMem = vertexData };
			var vertexBuffer = default(ComPtr<ID3D11Buffer>);
			SilkMarshal.ThrowHResult(device.CreateBuffer(in bufferDesc, in subresourceData, ref vertexBuffer));
			return vertexBuffer;
		}
	}

	private static unsafe ComPtr<ID3D11Buffer> CreateIndexBuffer(ComPtr<ID3D11Device> device, uint[] indices)
	{
		var bufferDesc = new BufferDesc
		{
			ByteWidth = (uint)(indices.Length * sizeof(uint)),
			Usage = Usage.Default,
			BindFlags = (uint)BindFlag.IndexBuffer
		};

		fixed (uint* indexData = indices)
		{
			var subresourceData = new SubresourceData { PSysMem = indexData };
			var indexBuffer = default(ComPtr<ID3D11Buffer>);
			SilkMarshal.ThrowHResult(device.CreateBuffer(in bufferDesc, in subresourceData, ref indexBuffer));
			return indexBuffer;
		}
	}

	private static unsafe (ComPtr<ID3D11VertexShader>, ComPtr<ID3D11PixelShader>, ComPtr<ID3D11InputLayout>)
		CompileShaders(ComPtr<ID3D11Device> device, string shaderSource, string inputSemanticName)
	{
		var compiler = D3DCompiler.GetApi();
		try
		{
			var shaderBytes = Encoding.ASCII.GetBytes(shaderSource);

			// Compile vertex shader
			var vertexCode = default(ComPtr<ID3D10Blob>);
			var vertexErrors = default(ComPtr<ID3D10Blob>);

			HResult hr = compiler.Compile(
				in shaderBytes[0],
				(nuint)shaderBytes.Length,
				nameof(shaderSource),
				null,
				ref Unsafe.NullRef<ID3DInclude>(),
				"vs_main",
				"vs_5_0",
				0,
				0,
				ref vertexCode,
				ref vertexErrors
			);

			if (hr.IsFailure)
			{
				if (vertexErrors.Handle is not null)
					throw new Exception(
						$"Vertex shader compilation failed: {SilkMarshal.PtrToString((nint)vertexErrors.GetBufferPointer())}");

				hr.Throw();
			}

			// Compile pixel shader
			var pixelCode = default(ComPtr<ID3D10Blob>);
			var pixelErrors = default(ComPtr<ID3D10Blob>);

			hr = compiler.Compile(
				in shaderBytes[0],
				(nuint)shaderBytes.Length,
				nameof(shaderSource),
				null,
				ref Unsafe.NullRef<ID3DInclude>(),
				"ps_main",
				"ps_5_0",
				0,
				0,
				ref pixelCode,
				ref pixelErrors
			);

			if (hr.IsFailure)
			{
				if (pixelErrors.Handle is not null)
					throw new Exception(
						$"Pixel shader compilation failed: {SilkMarshal.PtrToString((nint)pixelErrors.GetBufferPointer())}");

				hr.Throw();
			}

			// Create shaders
			var vertexShader = default(ComPtr<ID3D11VertexShader>);
			SilkMarshal.ThrowHResult(device.CreateVertexShader(vertexCode.GetBufferPointer(),
				vertexCode.GetBufferSize(),
				ref Unsafe.NullRef<ID3D11ClassLinkage>(), ref vertexShader)
			);

			var pixelShader = default(ComPtr<ID3D11PixelShader>);
			SilkMarshal.ThrowHResult(device.CreatePixelShader(pixelCode.GetBufferPointer(), pixelCode.GetBufferSize(),
				ref Unsafe.NullRef<ID3D11ClassLinkage>(), ref pixelShader)
			);

			// Create input layout
			var inputLayout = default(ComPtr<ID3D11InputLayout>);
			fixed (byte* name = SilkMarshal.StringToMemory(inputSemanticName))
			{
				var inputElement = new InputElementDesc
				{
					SemanticName = name,
					SemanticIndex = 0,
					Format = Format.FormatR32G32B32Float,
					InputSlot = 0,
					AlignedByteOffset = 0,
					InputSlotClass = InputClassification.PerVertexData,
					InstanceDataStepRate = 0
				};

				SilkMarshal.ThrowHResult(
					device.CreateInputLayout(in inputElement, 1, vertexCode.GetBufferPointer(),
						vertexCode.GetBufferSize(), ref inputLayout)
				);
			}

			vertexCode.Dispose();
			vertexErrors.Dispose();
			pixelCode.Dispose();
			pixelErrors.Dispose();

			return (vertexShader, pixelShader, inputLayout);
		}
		finally
		{
			compiler.Dispose();
		}
	}
}