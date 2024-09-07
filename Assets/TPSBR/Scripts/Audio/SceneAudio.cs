using UnityEngine;
using UnityEngine.Audio;

namespace TPSBR
{
	public class SceneAudio : SceneService 
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private AudioMixer _masterMixer;

		// PUBLIC METHODS
public void UpdateVolume()
{
    if (_masterMixer == null)
        return;

    // Update music volume
    float musicVolume = Context.RuntimeSettings.MusicVolume;
    float dbMusicVolume = musicVolume > 0 ? Mathf.Log10(musicVolume) * 20 : -144.0f; // Use -144 dB for silence

    _masterMixer.SetFloat("MusicVolume", dbMusicVolume);

    // Update effects volume
    float effectsVolume = Context.RuntimeSettings.EffectsVolume;
    float dbEffectsVolume = effectsVolume > 0 ? Mathf.Log10(effectsVolume) * 20 : -144.0f; // Use -144 dB for silence

    _masterMixer.SetFloat("EffectsVolume", dbEffectsVolume);
}

		// GameService INTERFACE

		protected override void OnActivate()
		{
			base.OnActivate();

			UpdateVolume();
		}
	}
}
