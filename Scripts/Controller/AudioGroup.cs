using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioGroup {

    [Tooltip("Use o nome como referência no script")]
    public string name;
    [Tooltip("Caminho exato do AudioGroup no Mixer. Ex.: UI/TrilhaSonora")]
    public string path;
}
