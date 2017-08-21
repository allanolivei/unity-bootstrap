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

    public void Empty()
    {
        this.queue.Clear();
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

    protected T objectReference;

    public UnityObjectPooling( T objectReference ) : base()
    {
        this.objectReference = objectReference;
    }

    public override T Create( params object[] args )
    {
        return UnityEngine.Object.Instantiate<T>(objectReference);
    }

    public override T Get( params object[] args )
    {
        T result = base.Get(args);
        result.name = objectReference.name;
        return result;
    }

    public virtual W GetComponent<W>( params object[] args ) where W : UnityEngine.Component
    {
        var element = this.Get(args) as W;
#if UNITY_EDITOR
        if( element == null )
            element = Create(args) as W;
#endif
        element.gameObject.SetActive(true);
        return element;
    }

    public virtual void RecycleComponent<W>( W component ) where W : UnityEngine.Component
    {
        component.gameObject.SetActive(false);
        this.Recycle( component as T );
    }

}