using System.Globalization;
using System.Text.RegularExpressions;
using NCalc;

namespace Mathematics.App.Maui.Systems.Sensors.Services;

public sealed partial class CalculatorService
{
	public async Task<double> EvaluateAsync(string input)
	{
		var exprText = Preprocess(input);

		var exp = new AsyncExpression(exprText)
		{
			Parameters =
			{
				["pi"] = Math.PI,
				["e"] = Math.E
			},
			Functions =
			{
				["sin"] = async a => Math.Sin(ToRad(await EvalAsync(a[0]))),
				["cos"] = async a => Math.Cos(ToRad(await EvalAsync(a[0]))),
				["tan"] = async a => Math.Tan(ToRad(await EvalAsync(a[0]))),
				["asin"] = async a => ToDeg(Math.Asin(await EvalAsync(a[0]))),
				["acos"] = async a => ToDeg(Math.Acos(await EvalAsync(a[0]))),
				["atan"] = async a => ToDeg(Math.Atan(await EvalAsync(a[0]))),
				["sqrt"] = async a => Math.Sqrt(await EvalAsync(a[0])),
				["log"] = async a => Math.Log10(await EvalAsync(a[0])),
				["ln"] = async a => Math.Log(await EvalAsync(a[0])),
				["exp"] = async a => Math.Exp(await EvalAsync(a[0])),
				["pow"] = async a => Math.Pow(await EvalAsync(a[0]), await EvalAsync(a[1]))
			}
		};

		var result = await exp.EvaluateAsync();
		return Convert.ToDouble(result, CultureInfo.InvariantCulture);
	}

	private static async Task<double> EvalAsync(AsyncExpression expr)
	{
		var val = await expr.EvaluateAsync();
		return Convert.ToDouble(val, CultureInfo.InvariantCulture);
	}

	private static string Preprocess(string expr)
	{
		expr = expr.Replace("π", "pi");
		expr = PowRegex().Replace(expr, "pow($1,$2)");
		expr = expr.Replace("Exp", "exp");
		return expr;
	}

	private static double ToRad(double deg) => deg * Math.PI / 180.0;
	private static double ToDeg(double rad) => rad * 180.0 / Math.PI;

	[GeneratedRegex(@"(\([^()]+\)|\d+(\.\d+)?)\^(\([^()]+\)|\d+(\.\d+)?)")]
	private static partial Regex PowRegex();
}