using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ParticleEffectManager : MonoBehaviour
{
    /*
     * The following script is used for the serial replication of a provided particle system.
     * The particle is able to be modified in the following ways:
     *  - The location where the particle will spawn
     *  - The start size of the particle
     *  - The start color of the particle
     *  - The layer the particle will be rendered on
     * For the purposes of this game, it will be used to generate a pulse-like particle as
     * requested by objects within the scene whenever they make "noise", creating an
     * audio-visualizer effect. It also will be used for generating blood splatters.
     */

    /*
     * These are the default variables used when one or several are omitted
     * on a call for a particle spawn using SpawnAudioEffect.
     */
    private static Vector3 DEFAULT_LOCATION = Vector3.zero; // Particles will spawn at the world origin if a location isn't specified
    private static float DEFAULT_SIZE = 1.0f; // Particles will spawn at a size of 1 if a size isn't specified
    private static Color DEFAULT_COLOR = Color.white; // Particles will not have any coloration if a color isn't specified
    private static int DEFAULT_LAYER = 0; // Particle will render on the default layer if an owner isn't specified

    /*
     * This is the template particle that all of the clones will be generated from.
     * This must be assigned in the inspector for the script to work.
     */
    [Header("Particle Template")]
    [SerializeField] private ParticleSystem particleSystemTemplate = null; // The Template Particle that will be cloned

    /*
     * These variables are for testing purposes. ParticleEffectManager Components contain
     * a specialized button that will request a particle with the parameters given by these
     * test variables during runtime. (ParticleEffectManagerEditor.cs overrides the inspector for
     * instances of this Component to produce this button)
     */
    [Header("Test Particle Variables")]
    [SerializeField] private Vector3 testLocation = DEFAULT_LOCATION; // The location of the Test Particle
    [SerializeField] private float testSize = DEFAULT_SIZE; // The size of the Test Particle
    [SerializeField] private Color testColor = DEFAULT_COLOR; // The color of the Test Particle
    [SerializeField] private int testLayer = DEFAULT_LAYER; // The render layer of the Test Particle

    // Spawn a particle using the Test Variables
    public void TestEffect()
    {
        SpawnAudioEffect(testLocation, testSize, testColor, testLayer);
    }

    // Spawn particle with default attributes
    public void SpawnAudioEffect()
    {
        SpawnAudioEffect(DEFAULT_LOCATION, DEFAULT_SIZE, DEFAULT_COLOR, DEFAULT_LAYER);
    }

    // Spawn particle at given location
    public void SpawnAudioEffect(Vector3 location)
    {
        SpawnAudioEffect(location, DEFAULT_SIZE, DEFAULT_COLOR, DEFAULT_LAYER);
    }

    // Spawn particle with given size
    public void SpawnAudioEffect(float size)
    {
        SpawnAudioEffect(DEFAULT_LOCATION, size, DEFAULT_COLOR, DEFAULT_LAYER);
    }

    // Spawn particle with given color
    public void SpawnAudioEffect(Color color)
    {
        SpawnAudioEffect(DEFAULT_LOCATION, DEFAULT_SIZE, color, DEFAULT_LAYER);
    }

    // Spawn particle on the given render layer
    public void SpawnAudioEffect(int layer)
    {
        SpawnAudioEffect(DEFAULT_LOCATION, DEFAULT_SIZE, DEFAULT_COLOR, layer);
    }

    // Spawn particle at given location with given size
    public void SpawnAudioEffect(Vector3 location, float size)
    {
        SpawnAudioEffect(location, size, DEFAULT_COLOR, DEFAULT_LAYER);
    }

    // Spawn particle at given location with given color
    public void SpawnAudioEffect(Vector3 location, Color color)
    {
        SpawnAudioEffect(location, DEFAULT_SIZE, color, DEFAULT_LAYER);
    }

    // Spawn particle at given location on the given render layer
    public void SpawnAudioEffect(Vector3 location, int layer)
    {
        SpawnAudioEffect(location, DEFAULT_SIZE, DEFAULT_COLOR, layer);
    }

    // Spawn particle at given location with the given size and color
    public void SpawnAudioEffect(Vector3 location, float size, Color color)
    {
        SpawnAudioEffect(location, size, color, DEFAULT_LAYER);
    }

    // Spawn particle at given location with the given size on the given render layer
    public void SpawnAudioEffect(Vector3 location, float size, int layer)
    {
        SpawnAudioEffect(location, size, DEFAULT_COLOR, layer);
    }

    // Spawn particle at given location with the given size and color on the given render layer
    public ParticleSystem SpawnAudioEffect(Vector3 location, float size, Color color, int layer)
    {
        ParticleSystem ps = Instantiate(particleSystemTemplate, location, Quaternion.identity); // Instantiate a clone of the Template Particle
        var main = ps.main; // Variable which allows the Particle System Main Module of the clone to be modified
        main.startSize = size; // Set the start size of the particle clone
        main.startColor = color; // Set the start color of the particle clone
        ps.gameObject.layer = layer; // Set the render layer of the particle clone
        return ps;
    }

}