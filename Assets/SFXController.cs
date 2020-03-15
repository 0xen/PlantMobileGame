using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXController : MonoBehaviour
{
    [System.Serializable]
    public struct SFX
    {
        public string name;
        public AudioClip source;
    }
    public SFX[] sfxs;

    private Dictionary<string, AudioClip> effects;

    public int channelCount;
    public AudioMixerGroup mixer;


    public static SFXController instance;

    private AudioSource[] audioChannels;
    // Start is called before the first frame update
    void Start()
    {
        effects = new Dictionary<string, AudioClip>();
        foreach(SFX s in sfxs)
        {
            effects[s.name] = s.source;
        }
        instance = this;
        audioChannels = new AudioSource[channelCount];
        for (int i = 0; i<channelCount;i++)
        {
            GameObject obj = new GameObject();
            obj.transform.name = "Channel";
            obj.transform.parent = this.transform;
            audioChannels[i] = obj.AddComponent<AudioSource>();
            audioChannels[i].outputAudioMixerGroup = mixer;
        }
    }

    public void Play(string name)
    {
        if (!effects.ContainsKey(name)) return;
        for (int i = 0; i < channelCount; i++)
        {
            if(!audioChannels[i].isPlaying)
            {
                audioChannels[i].clip = effects[name];
                audioChannels[i].Play();
                return;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
