using System;

namespace LinkedStack
{
	public class LinkedStackItem<T>
	{
		public T Value { get; private set; }
		public LinkedStackItem<T> Previous { get; set; }

		public LinkedStackItem(T value)
		{
			Value = value;
			Previous = null;
		}
	}
	
	public class LinkedStack<T>
	{
		public LinkedStackItem<T> Last;
		public int Count { get; private set; }

		public void Push(T element)
	    {
		    var next = new LinkedStackItem<T>(element);
		    if (Last != null)
			    next.Previous = Last;

		    Last = next;
		    Count++;
	    }

	    public T Pop()
	    {
		    Count--;
		    if (Last == null)
			    throw new InvalidOperationException();

		    var result = Last.Value;
		    Last = Last.Previous;
		    return result;
	    }

	    public T Peek()
	    {
		    return Last.Value;
	    }
	    
	    public void Clear()
	    {
		    Count = 0;
		    Last = null;
	    }

	    public LinkedStack<T> Clone()
	    {
		    return new LinkedStack<T> { Last = Last, Count = Count };
	    }
	}

	class Program
    {
        public static void Main()
        {
            var stack = new LinkedStack<int>();
            stack.Push(5);
            stack.Push(4);
            Console.WriteLine(stack.Pop());
        }
    }
}
