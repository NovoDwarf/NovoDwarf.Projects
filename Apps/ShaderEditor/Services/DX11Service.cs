using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using Messager.NET.Entity.Resources;
using Messager.NET.Interfaces.Receivers;
using Messager.NET.Interfaces.Senders;
using Microsoft.Extensions.Logging;
using ShaderEditor.Events;
using ShaderEditor.Extensions;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D.Compilers;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace ShaderEditor.Services;

public sealed class DX11Service
{
	private readonly ILogger<DX11Service> _logger;
	
	private readonly List<MeshRenderRequest> _meshRenderQueue = [];
	private readonly List<ShaderRenderRequest> _shaderRenderQueue = [];
	
	private readonly DisposableList _disposables = new();
	
	public DX11Service(
		ILogger<DX11Service> logger,
		IReceiver<MeshRenderRequest> meshRenderReceiver,
		IReceiver<ShaderRenderRequest> shaderRenderReceiver,
		IReceiver<WindowLoadEvent> loadReceiver,
		IReceiver<WindowUpdateEvent> updateReceiver,
		IReceiver<WindowRenderEvent> renderReceiver,
		IReceiver<WindowResizeEvent> resizeReceiver,
		IReceiver<WindowCloseEvent> closeReceiver)
	{
		_logger = logger;
		
		var load = loadReceiver.Subscribe(OnLoad);
		var update = updateReceiver.Subscribe(OnUpdate);
		var resize = resizeReceiver.Subscribe(OnResize);
		var compiled = renderReceiver.Subscribe(OnRender);
		var close = closeReceiver.Subscribe(OnClose);
		var mesh = meshRenderReceiver.Subscribe(OnMeshRenderRequest);
		var shader = shaderRenderReceiver.Subscribe(OnShaderRenderRequest);
		
		_disposables.Add(load, update, resize, close, compiled, mesh, shader);
	}
	
	private IWindow _window = null!;
	
	private DXGI _dxgi = null!;
	private D3D11 _d3d11 = null!;
	
	private ComPtr<IDXGIFactory2> _factory;
	private ComPtr<IDXGISwapChain1> _swapchain;
	
	private ComPtr<ID3D11Device> _device;
	private ComPtr<ID3D11DeviceContext> _deviceContext;

	private ComPtr<ID3D11RenderTargetView> _renderTargetView;

	private float[] _backgroundColour = [0.0f, 0.0f, 0.0f, 1.0f];
	
	private void OnLoad(WindowLoadEvent evt)
	{
		_window = evt.Window;
		_dxgi = DXGI.GetApi(evt.Window);
		_d3d11 = D3D11.GetApi(evt.Window);

		CreateLogicalDevice();
		CreateSwapchain();
		CreateRenderTargetView();
	}

	private void OnResize(WindowResizeEvent evt)
    {
	    var newSize = evt.Size;

	    _renderTargetView.Dispose();

	    SilkMarshal.ThrowHResult(
		    _swapchain.ResizeBuffers(0, (uint)newSize.X, (uint)newSize.Y, Format.FormatB8G8R8A8Unorm, 0)
	    );

	    CreateRenderTargetView();
    }
	
	private void OnRender(WindowRenderEvent evt)
	{
		ClearRenderTarget();
		SetViewport();
        
		ProcessShaderRenderRequests();
		ProcessMeshRenderRequests();

		_swapchain.Present(1, 0);
		
		_meshRenderQueue.Clear();
		_shaderRenderQueue.Clear();
	}
	
	private void OnUpdate(WindowUpdateEvent evt)
	{
		
	}
	
	private void OnClose(WindowCloseEvent evt)
	{
		_factory.Dispose();
		_swapchain.Dispose();
		
		_device.Dispose();
		_deviceContext.Dispose();
		
		_d3d11.Dispose();
		_dxgi.Dispose();
	}

	private void OnMeshRenderRequest(MeshRenderRequest request)
	{
		_meshRenderQueue.Add(request);
	}

	private void OnShaderRenderRequest(ShaderRenderRequest request)
	{
		_shaderRenderQueue.Add(request);
	}

	
	private unsafe void CreateLogicalDevice()
	{
		SilkMarshal.ThrowHResult
		(
			_d3d11.CreateDevice(default(ComPtr<IDXGIAdapter>), D3DDriverType.Hardware, Software: 0, (uint) CreateDeviceFlag.Debug,
				null, 0, D3D11.SdkVersion, ref _device, null, ref _deviceContext)
		);
	}

	private unsafe void CreateSwapchain()
	{
		var swapChainDesc = new SwapChainDesc1
		{
			BufferCount = 2,
			Format = Format.FormatB8G8R8A8Unorm,
			BufferUsage = DXGI.UsageRenderTargetOutput,
			SwapEffect = SwapEffect.FlipDiscard,
			SampleDesc = new SampleDesc(1, 0)
		};
		
		_factory = _dxgi.CreateDXGIFactory<IDXGIFactory2>();

		SilkMarshal.ThrowHResult
		(
			_factory.CreateSwapChainForHwnd(_device, _window.Native!.DXHandle!.Value, in swapChainDesc,
				null, ref Unsafe.NullRef<IDXGIOutput>(), ref _swapchain)
		);
	}
	
	private void SetViewport()
	{
		var viewport = new Viewport(0, 0, _window.FramebufferSize.X, _window.FramebufferSize.Y, 0, 1);
		
		_deviceContext.RSSetViewports(1, in viewport);
		_deviceContext.OMSetRenderTargets(1, ref _renderTargetView, ref Unsafe.NullRef<ID3D11DepthStencilView>());
	}
	
	private void ProcessShaderRenderRequests()
	{
		foreach (var request in _shaderRenderQueue)
		{
			try
			{
				request.Execute(_device, _deviceContext);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to execute shader render request");
			}
		}
	}

	private void ProcessMeshRenderRequests()
	{
		foreach (var request in _meshRenderQueue)
		{
			try
			{
				request.Execute(_deviceContext);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to execute mesh render request");
			}
		}
	}
	
	private unsafe void CreateRenderTargetView()
	{
		var framebuffer = _swapchain.GetBuffer<ID3D11Texture2D>(0);
		
		SilkMarshal.ThrowHResult(_device.CreateRenderTargetView(framebuffer, null, ref _renderTargetView));
	}
	
	private void ClearRenderTarget()
	{
		_deviceContext.ClearRenderTargetView(_renderTargetView, ref _backgroundColour[0]);
	}
}