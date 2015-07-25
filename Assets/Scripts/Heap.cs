using UnityEngine;
using System.Collections;
using System;

public class Heap<T> where T : IHeapItem<T>{

	private T[] items;
	private int currentItemCount;
	
	// Constructor
	public Heap(int maxHeapSize){
		items = new T[maxHeapSize];
	}

	/*********************************************************************************/
	/*	Public Functions - Order: Alphabetic										 */		
	/*********************************************************************************/

	public void Add(T item){
		item.setHeapIndex(currentItemCount);
		items[currentItemCount] = item;
		SortUp (item);
		currentItemCount++;
	}

	public bool Contains(T item){
		return Equals (items[item.getHeapIndex()], item);
	}

	public T RemoveFirst(){
		T firstItem = items[0];
		currentItemCount--;
		items[0] = items[currentItemCount];
		items[0].setHeapIndex(0);
		SortDown(items[0]);
		return firstItem;
	}

	public void UpdateItem(T item){
		SortUp (item);
	}

	/*********************************************************************************/
	/*	Private Functions - Order: Alphabetic										 */		
	/*********************************************************************************/	

	private void SortDown(T item){
		while(true){
			int childIndexLeft = item.getHeapIndex() * 2 + 1;
			int childIndexRight = item.getHeapIndex() * 2 + 2;
			int swapIndex = 0;

			if(childIndexLeft < currentItemCount){
				swapIndex = childIndexLeft;

				if(childIndexRight < currentItemCount){
					if(items[childIndexLeft].CompareTo(items[childIndexRight]) < 0){
						swapIndex = childIndexRight;
					}
				}

				if(item.CompareTo(items[swapIndex]) < 0){
					Swap (item,items[swapIndex]);
				}else{
					return;
				}
			}else{
				return;
			}
		}
	}

	private void SortUp(T item){
		int parentIndex = (item.getHeapIndex()-1)/2;

		while(true){
			T parentItem = items[parentIndex];
			if(item.CompareTo (parentItem) > 0){
				Swap (item,parentItem);
			}else{
				break;
			}
			parentIndex = (item.getHeapIndex()-1)/2;
		}
	}

	private void Swap(T itemA, T itemB){
		items[itemA.getHeapIndex()] = itemB;
		items[itemB.getHeapIndex()] = itemA;
		int itemAIndex = itemA.getHeapIndex();
		itemA.setHeapIndex(itemB.getHeapIndex());
		itemB.setHeapIndex(itemAIndex);
	}

	/*********************************************************************************/
	/*	Getter and Setter Functions - Order: Alphabetic							 	 */		
	/*********************************************************************************/

	public int getCurrentItemCount() {
		return this.currentItemCount;
	}
}