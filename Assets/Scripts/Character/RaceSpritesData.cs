using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/RaceSpritesData")]
public class RaceSpritesData : ScriptableObject {
    [Header("Tail")]
    public Sprite fillTail;
    public Sprite lineTail;
    [Header("Head")]
    public Sprite fillHead;
    public Sprite fillFace;
    public Sprite lineHead;
}
