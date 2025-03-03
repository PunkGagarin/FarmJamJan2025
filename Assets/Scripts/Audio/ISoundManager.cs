namespace Farm.Audio
{
    public interface ISoundManager
    {
        void PlaySoundByType(GameAudioType type, int soundIndex);
    }
}