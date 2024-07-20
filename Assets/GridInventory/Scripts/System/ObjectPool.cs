using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ObjectPool<T>
{
    public delegate void ReturnItem(T item);
    public event ReturnItem OnReturnItemEvent;
    public delegate T TakeItem(Transform transform);
    public event TakeItem OnTakeItemEvent;

    Stack<T> stack;
    Func<T> createItemFunc; // Insert Creation Method on pooler script

    public ObjectPool(Func<T> OnCreateItem, int maxAmount = 10)
    {
        stack = new Stack<T>(maxAmount);
        this.createItemFunc = OnCreateItem;
        for (int i = 0; i < maxAmount; i++)
        {
            T thing = OnCreateItem();

            stack.Push(thing);
        }
    }

    public T Take()
    {
        T item;
        if (stack.Count > 0)
        {
            item = stack.Pop();
        }
        else
        {
            item = createItemFunc();
        }

        return item;
    }
    public void Return(T item)
    {

        stack.Push(item);

    }



}
