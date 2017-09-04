// Copyright (c) 2017-2018 Allan Oliveira Marinho(allanolivei@gmail.com), Inc. All Rights Reserved. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : Singleton<ParticleManager>
{

    [SerializeField]
    private List<ParticleSystem> prefabs = new List<ParticleSystem>();
    private List<List<ParticleSystem>> all = new List<List<ParticleSystem>>();

    public void Register( ParticleSystem particle )
    {
        prefabs.Add(particle);
        all.Add(new List<ParticleSystem>());
    }

    public ParticleSystem Show( string partName, Vector3 position )
    {
        int index = FindIndex(partName);
        if (index == -1) return null;

        ParticleSystem p = FindFreeParticle(index);
        if( p == null )
        {
            p = Instantiate<ParticleSystem>(prefabs[index]);
            p.name = prefabs[index].name;
            all[index].Add(p);
        }
        p.transform.position = position;
        p.Play();
        return p;
    }

    private ParticleSystem FindFreeParticle( int index )
    {
        for (int i = 0 ; i < all[index].Count ; i++)
            if (!all[index][i].IsAlive())
                return all[index][i];
        return null;
    }

    private int FindIndex( string partName )
    {
        for (int i = 0 ; i < prefabs.Count ; i++)
            if ( prefabs[i].name.Contains(partName) ) return i;

        return -1;
    }
    
    protected override void Awake()
    {
        base.Awake();

        for( int i = 0 ; i < prefabs.Count ; i++ )
            all.Add( new List<ParticleSystem>() );
    }
	
}
