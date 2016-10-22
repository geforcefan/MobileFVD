using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FVD.Model
{
	public class Function
	{
		public int activeSubFunction = -1;

		public FunctionType type;
		public Section section;

		public float startValue;

		public ObservableCollection<SubFunction> subFunctions = new ObservableCollection<SubFunction>();

		public Action<Function> OnFunctionChanged;

		public Function(float min, float max, float _startValue, float end, Section _section, FunctionType _type)
		{
			type = _type;
			section = _section;
			startValue = _startValue;

			subFunctions.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => {
				if(OnFunctionChanged != null)
					OnFunctionChanged(this);
			};

			subFunctions.Add(new SubFunction(min, max, startValue, end - startValue, this));
		}

		public float getValue(float x)
		{
			int i = 0;
			int s = subFunctions.Count;
			SubFunction cur = null;
			for (; i < s; i++)
			{
				cur = subFunctions[i];
				if (cur.maxArgument >= x)
				{
					break;
				}
			}

			return cur.getValue(x);
		}

		public void appendSubFunction(float length, int i)
		{
			int index = subFunctions.Count;
			SubFunction temp, prev;

			if (i == -1)
			{
				if (index == 0)
				{
					temp = new SubFunction(0.0f, 1.0f, startValue, 0.0f, this);
				}
				else {
					prev = subFunctions.First();
					temp = new SubFunction(0.0f, length, prev.startValue, 0.0f, this);
				}
				subFunctions.Insert(0, temp);
				activeSubFunction = index;
			}
			else {
				SubFunction pred = subFunctions[i];
				subFunctions.Insert(i + 1, new SubFunction(pred.maxArgument, pred.maxArgument + length, pred.endValue(), 0.0f, this));
				activeSubFunction = i + 1;
			}

			int s = subFunctions.Count;
			for (i = 1; i < s; ++i)
			{
				prev = subFunctions[i - 1];
				SubFunction cur = subFunctions[i];
				cur.update(prev.maxArgument, prev.maxArgument + cur.maxArgument - cur.minArgument, cur.symArg);
			}
		}

		public void removeSubFunction(int i)
		{
			int index = subFunctions.Count;

			if (index <= 1)
			{
				return;
			}

			// TODO: DISPOSE
			//delete funcList[i];
			subFunctions.RemoveAt(i);

			SubFunction cur;
			if (i == 0)
			{
				cur = subFunctions[i];
				cur.update(0, cur.maxArgument - cur.minArgument, cur.symArg);
				++i;
			}
			for (; i < subFunctions.Count; ++i)
			{
				SubFunction prev = subFunctions[i - 1];
				cur = subFunctions[i];
				translateValues(prev);
				cur.update(prev.maxArgument, prev.maxArgument + cur.maxArgument - cur.minArgument, cur.symArg);
			}
		}

		public void setMaxArgument(float newMax)
		{
			float scale = newMax / maxArgument;
			for (int i = 0; i < subFunctions.Count; i++)
			{
				SubFunction cur = subFunctions[i];
				cur.update(cur.minArgument * scale, cur.maxArgument * scale, cur.symArg);
			}
		}


		public float maxArgument
		{
			get
			{
				return subFunctions.Last().maxArgument;
			}
		}

		public void translateValues(SubFunction caller)
		{
			int i = 0;
			SubFunction prev = null, cur = null;
			while (i < subFunctions.Count)
			{
				cur = subFunctions[i++];
				if (cur == caller) break;
			}

			for (; i < subFunctions.Count; ++i)
			{
				prev = cur;
				cur = subFunctions[i];
				cur.translateValues(prev.endValue());
			}
		}

		public float changeLength(float newlength, int index)
		{
			SubFunction cur = subFunctions[index];
			SubFunction prev;

			cur.update(cur.minArgument, cur.minArgument + newlength, cur.symArg);
			for (++index; index < subFunctions.Count; ++index)
			{
				prev = cur;
				cur = subFunctions[index];
				if (cur.locked)
				{
					cur.update(prev.maxArgument, section.getMaxArgument(), cur.symArg);
				}
				else {
					cur.update(prev.maxArgument, prev.maxArgument + cur.maxArgument - cur.minArgument, cur.symArg);
				}
			}
			return maxArgument;
		}

		public int getSubFunctionIndex(SubFunction sub)
		{
			int number = 0;

			while (subFunctions[number] != sub && number < subFunctions.Count) ++number;
			if (number < subFunctions.Count)
			{
				return number;
			}
			else {
				return -1;
			}
		}

		public void lockFunction(int index)
		{
			subFunctions[index].locked = true;
		}

		public void unlockFunction(int index)
		{
			subFunctions[index].locked = false;
		}

		public int lockedFunction
		{
			get
			{
				for (int i = 0; i < subFunctions.Count; ++i)
				{
					if (subFunctions[i].locked) return i;
				}
				return -1;
			}
		}

		public SubFunction getSubfunction(float x)
		{
			int i = 0;
			SubFunction cur = null;

			int s = subFunctions.Count;
			for (; i < s; ++i)
			{
				cur = subFunctions[i];
				if (cur.maxArgument >= x)
				{
					break;
				}
			}
			return cur;
		}
	}
}