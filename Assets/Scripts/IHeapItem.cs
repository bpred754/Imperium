using System;

public interface IHeapItem<T> : IComparable<T> {

	int getHeapIndex();
	void setHeapIndex(int _heapIndex);
}

