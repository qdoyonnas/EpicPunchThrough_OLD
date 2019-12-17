using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anima2D;

[CreateAssetMenu(menuName = "Project/CharacterSkin", order = 1)]
public class CharacterSkin : ScriptableObject
{
    public new string name;

    [Header("Body")]
    public SpriteMesh Face;
    public SpriteMesh Chest;
    public SpriteMesh Hip;

    [Header("Left Arm")]
    public SpriteMesh LeftShoulder;
    public SpriteMesh LeftArm;
    public SpriteMesh LeftHand;
    public SpriteMesh Fingers;

    [Header("Right Arm")]
    public SpriteMesh RightShoulder;
    public SpriteMesh RightArm;
    public SpriteMesh RightHand;

    [Header("Left Leg")]
    public SpriteMesh LeftThigh;
    public SpriteMesh LeftLeg;
    public SpriteMesh LeftFoot;

    [Header("Right Leg")]
    public SpriteMesh RightThigh;
    public SpriteMesh RightLeg;
    public SpriteMesh RightFoot;
}
