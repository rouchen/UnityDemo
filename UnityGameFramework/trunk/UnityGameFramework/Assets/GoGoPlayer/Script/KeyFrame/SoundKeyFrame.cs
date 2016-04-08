using UnityEngine;
using System.Collections;

[System.Serializable]
public class SoundKeyFrame : KeyFrameBase
{
    public enum PlayType
    {
        NONE,
        PLAY,
        RESUME,
        PAUSE,
        STOP,
    };

    public PlayType playType;
    public float volume;

    public SoundKeyFrame() : base()
    {
        playType = PlayType.NONE;
        volume = 0.5f;
    }
}
