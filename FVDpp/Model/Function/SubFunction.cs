using System;
using GlmNet;

namespace FVD.Model
{
	public class SubFunction
	{
		public float minArgument;
		public float maxArgument;

		public float startValue;

		public float arg1;
		public float symArg;

		public bool locked;

		public float centerArg;
		public float tensionArg;

		public FunctionDegree degree;
		public Function function;

		public SubFunction(float min, float max, float start, float diff, Function _function)
		{
			function = _function;

    		minArgument = min;
			maxArgument = max;

			centerArg = 0.0f;
			tensionArg = 0.0f;
			symArg = diff;

			startValue = start;
			function = _function;

			if (function.type == FunctionType.Normal)
			{
				changeDegree(FunctionDegree.Cubic);
			}
			else {
				changeDegree(FunctionDegree.Quartic);
			}

			locked = false;
		}

		public void update(float min, float max, float diff)
		{
			minArgument = min;
			maxArgument = max;

			symArg = diff;

			function.translateValues(this);
		}

		public void changeDegree(FunctionDegree newDegree)
		{
			degree = newDegree;

			switch (newDegree)
			{
				case FunctionDegree.Linear:
					break;
				case FunctionDegree.Quadratic:
					break;
				case FunctionDegree.Cubic:
					break;
				case FunctionDegree.Quartic:
					arg1 = -10.0f;
					break;
				case FunctionDegree.Quintic:
					arg1 = 0.0f;
					break;
				case FunctionDegree.Sinusoidal:
					break;
				case FunctionDegree.Plateau:
					arg1 = 1.0f;
					break;
				case FunctionDegree.FreeForm:
					break;
				case FunctionDegree.ToZero:
					centerArg = 0.0f;
					tensionArg = 0.0f;
					symArg = -startValue;
					break;
				default:
					break;
			}

			return;
		}

		public void changeLength(float len)
		{
			function.changeLength(len, function.getSubFunctionIndex(this));
		}

		public float getValue(float x)
		{
			if (locked)
			{
				function.changeLength(function.section.getMaxArgument() - minArgument, function.getSubFunctionIndex(this));
			}
			else if (x > maxArgument)
			{
				x = maxArgument;
			}
			else if (x < minArgument)
			{
				x = minArgument;
			}

			x = (x - minArgument) / (maxArgument - minArgument);

			x = applyCenter(x);
			x = applyTension(x);

			float root;
			float max;

			switch (degree)
			{
				case FunctionDegree.Linear:
					return symArg * x + startValue;
				case FunctionDegree.Quadratic:
					if (isSymmetric())
					{
						x = 2.0f * x - 1.0f;
						return symArg * (1.0f - x * x) + startValue;
					}
					else if (arg1 < 0.0f)
					{
						return symArg * (1.0f - (1.0f - x) * (1.0f - x)) + startValue;
					}
					else
					{
						return symArg * x * x + startValue;
					}
				case FunctionDegree.Cubic:
					return symArg * x * x * (3.0f + x * (-2.0f)) + startValue;
				case FunctionDegree.Quartic:
					if (!isSymmetric())
					{
						return x * x * (-(6.0f * symArg * arg1) / (1.0f - 2.0f * arg1) + x * (symArg * (4.0f * arg1 + 4.0f) / (1.0f - 2.0f * arg1) + x * ((-3.0f * symArg / (1.0f - 2.0f * arg1))))) + startValue;
					}
					else
					{
						return symArg * x * x * (16.0f + x * (-32.0f + x * 16.0f)) + startValue;
					}
				case FunctionDegree.Quintic:
					if (glm.abs(arg1) < 0.005f)
					{
						return symArg * x * x * x * (10.0f + x * (-15.0f + x * 6.0f)) + startValue;
					}
					else if (arg1 < 0.0f)
					{
						root = -glm.sqrt(9.0f + glm.abs(arg1 / 10.0f) * (-16.0f + 16.0f * glm.abs(arg1 / 10.0f)));
						max = 0.01728f + 0.00576f * root + glm.abs(arg1 / 10.0f) * (-0.0288f - 0.00448f * root + glm.abs(arg1 / 10.0f) * (0.0032f - 0.00576f * root + glm.abs(arg1 / 10.0f) * (-0.0704f + 0.02048f * root + glm.abs(arg1 / 10.0f) * (0.1024f - 0.01024f * root + arg1 / 10.0f * 0.04096f))));
						return symArg / max * x * x * (x - 1.0f) * (x - 1.0f) * (x + arg1 / 10.0f) + startValue;
					}
					else
					{
						root = glm.sqrt(9.0f + arg1 / 10.0f * (-16.0f + 16.0f * arg1 / 10.0f));
						max = 0.01728f + 0.00576f * root + arg1 / 10.0f * (-0.0288f - 0.00448f * root + arg1 / 10.0f * (0.0032f - 0.00576f * root + arg1 / 10.0f * (-0.0704f + 0.02048f * root + arg1 / 10.0f * (0.1024f - 0.01024f * root - arg1 / 10.0f * 0.04096f))));
						return symArg / max * x * x * (x - 1.0f) * (x - 1.0f) * (x - arg1 / 10.0f) + startValue;
					}
				case FunctionDegree.Sinusoidal:
					return 0.5f * symArg * (1 - glm.cos((float)Math.PI * x)) + startValue;
				case FunctionDegree.Plateau:
					return symArg * (1.0f - ((float)Math.Exp(-arg1 * 15.0f * ((float)Math.Pow(1.0f - glm.abs(2.0f * x - 1.0f), 3.0f))))) + startValue;
			}
			return -1;
		}

		public float getMinValue()
		{
			return startValue < endValue() ? startValue : endValue();
		}

		public float getMaxValue()
		{
			return startValue > endValue() ? startValue : endValue();

		}

		public void translateValues(float newStart)
		{
			startValue = newStart;
			if (degree == FunctionDegree.ToZero)
			{
				symArg = -startValue;
			}
		}

		public bool isSymmetric()
		{
			if (degree == FunctionDegree.Quadratic && glm.abs(arg1) < 0.5f)
				return true;
			if (degree == FunctionDegree.Quartic && arg1 < 0.0f)
				return true;
			if (degree == FunctionDegree.Quintic && glm.abs(arg1) > 0.005f)
				return true;
			if (degree == FunctionDegree.Plateau)
				return true;
			return false;
		}

		public float applyTension(float x)
		{
			if (glm.abs(tensionArg) < 0.0005f)
			{
				return x;
			}
			else if (tensionArg > 0.0f)
			{
				x = 2.0f * tensionArg * (x - 0.5f);
				x = glm.sinh(x) / glm.sinh(tensionArg);
				x = 0.5f * (x + 1.0f);
			}
			else
			{
				x = 2.0f * glm.sinh(tensionArg) * (x - 0.5f);
				x = glm.asinh(x) / tensionArg;
				x = 0.5f * (x + 1.0f);
			}
			return x;
		}

		public float applyCenter(float x)
		{
			if (centerArg > 0.0f)
			{
				x = (float)Math.Pow(x, Math.Pow(2.0f, centerArg / 2.0f));
			}
			else if (centerArg < 0.0f)
			{
				x = 1.0f - (float)Math.Pow(1.0f - x, Math.Pow(2.0f, -centerArg / 2.0f));
			}
			return x;
		}

		public float endValue()
		{
			if (isSymmetric())
			{
				return startValue;
			}
			else {
				return startValue + symArg;
			}
		}
	}
}
