using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using NUnit.Framework;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> levelSprites; // Danh sách sprite cho các level
    [SerializeField]
    private Image currentLevelImage;
    [SerializeField]
    private Image nextLevelImage; 
    [SerializeField]
    private Image CompleteLevelImage; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
