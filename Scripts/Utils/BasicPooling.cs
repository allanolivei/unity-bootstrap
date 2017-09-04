// Copyright (c) 2017-2018 Allan Oliveira Marinho(allanolivei@gmail.com), Inc. All Rights Reserved. 

using System;
using System.Collections.Generic;

public class BasicPooling<T>
{
    protected Queue<T> queue;

    public BasicPooling()
    {
        this.queue = new Queue<T>();
    }

    public int Count
    {
        get { return queue.Count; }
    }

    public virtual T Get( params object[] args )
    {
        if (queue.Count > 0)
            return queue.Dequeue();
        return Create(args);
    }

    public virtual void Recycle( T element )
    {
        queue.Enqueue(element);
    }

    public virtual T Create( params object[] args )
    {
        return (T)Activator.CreateInstance(typeof(T), args);
    }
}

public class UnityObjectPooling<T> : BasicPooling<T> where T : UnityEngine.Object
{

    public T objectReference { get; protected set; }

    public UnityObjectPooling( T objectReference ) : base()
    {
        this.objectReference = objectReference;
    }

    public override T Create( params object[] args )
    {
        T inst = UnityEngine.Object.Instantiate<T>(objectReference);
        inst.name = objectReference.name;
        return inst;
    }

}