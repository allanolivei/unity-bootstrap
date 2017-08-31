using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MayaPostProcessor : AssetPostprocessor
{
    void OnPreprocessModel ()
    {
        if (assetPath.Contains("Maya"))
        {
            ModelImporter modelImporter = assetImporter as ModelImporter;
            modelImporter.materialName = ModelImporterMaterialName.BasedOnMaterialName;
        }
    }
}
