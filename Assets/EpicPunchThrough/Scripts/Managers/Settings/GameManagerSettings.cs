using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Project/Settings/GameManagerSettings", order = 1)]
public class GameManagerSettings : ScriptableObject
{
    public float sceneTransitionFadeDuration;
    public WorldBounds cameraBounds;
    public MenuSettings menuSettings;
    public InputSettings inputSettings;
    public SoundSettings soundSettings;
    public PlaySettings playSettings;
    public AgentSettings agentSettings;
    public EnvironmentSettings environmentSettings;
    public ParticleSettings particleSettings;
    public PropSettings propSettings;
}
