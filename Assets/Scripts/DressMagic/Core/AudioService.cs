using UnityEngine;

namespace DressMagic.Core
{
    /// <summary>Música y efectos. Carga los clips desde Resources y aplica los volúmenes guardados.</summary>
    public sealed class AudioService : MonoBehaviour
    {
        AudioSource musicSource;
        AudioSource sfxSource;

        AudioClip musicClip;
        AudioClip clickClip;
        AudioClip unlockClip;
        AudioClip shutterClip;

        float musicVolume = 0.55f;
        float sfxVolume = 0.8f;

        public void Init(SaveData save)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.spatialBlend = 0f;

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.spatialBlend = 0f;

            musicClip = LoadFirst("DressMagic/Music/TemaPrincipal", "DressMagic/Music");
            clickClip = LoadFirst("DressMagic/Sfx/Click", "DressMagic/Sfx");
            unlockClip = LoadFirst("DressMagic/Sfx/Unlock", "DressMagic/Sfx");
            shutterClip = LoadFirst("DressMagic/Sfx/Shutter", "DressMagic/Sfx");

            if (clickClip == null) clickClip = MakeTone("Click", 0.09f, new[] { 1320f, 1760f });
            if (unlockClip == null) unlockClip = MakeArpeggio("Unlock", new[] { 784f, 988f, 1175f, 1568f });
            if (shutterClip == null) shutterClip = MakeTone("Shutter", 0.12f, new[] { 900f, 300f });
            if (musicClip == null) musicClip = MakeMusicLoop();

            Apply(save);
            PlayMusic();
        }

        static AudioClip LoadFirst(string path, string folder)
        {
            AudioClip clip = Resources.Load<AudioClip>(path);
            if (clip != null) return clip;

            AudioClip[] all = Resources.LoadAll<AudioClip>(folder);
            if (all == null || all.Length == 0) return null;

            string wanted = System.IO.Path.GetFileName(path).ToLowerInvariant();
            foreach (AudioClip c in all)
            {
                if (c != null && c.name.ToLowerInvariant().Contains(wanted)) return c;
            }
            return all[0];
        }

        public void Apply(SaveData save)
        {
            musicVolume = Mathf.Clamp01(save.music);
            sfxVolume = Mathf.Clamp01(save.sfx);
            if (musicSource != null) musicSource.volume = musicVolume * 0.65f;
            if (sfxSource != null) sfxSource.volume = sfxVolume;
        }

        public void PlayMusic()
        {
            if (musicSource == null || musicClip == null) return;
            if (musicSource.isPlaying && musicSource.clip == musicClip) return;
            musicSource.clip = musicClip;
            musicSource.Play();
        }

        public void StopMusic()
        {
            if (musicSource != null) musicSource.Stop();
        }

        public void Click()
        {
            PlaySfx(clickClip, 0.85f);
        }

        public void Unlock()
        {
            PlaySfx(unlockClip, 1f);
        }

        public void Shutter()
        {
            PlaySfx(shutterClip, 0.9f);
        }

        void PlaySfx(AudioClip clip, float scale)
        {
            if (sfxSource == null || clip == null) return;
            sfxSource.PlayOneShot(clip, Mathf.Clamp01(sfxVolume * scale));
        }

        // ------------------------------------------------ respaldo procedural

        static AudioClip MakeTone(string name, float dur, float[] freqs)
        {
            int rate = 44100;
            int n = Mathf.CeilToInt(rate * dur);
            float[] data = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)rate;
                float env = Mathf.Exp(-t * 26f);
                float v = 0f;
                for (int f = 0; f < freqs.Length; f++)
                    v += Mathf.Sin(2f * Mathf.PI * freqs[f] * t) * (1f - f / (float)freqs.Length) * 0.5f;
                data[i] = v * env * 0.4f;
            }
            AudioClip clip = AudioClip.Create(name, n, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }

        static AudioClip MakeArpeggio(string name, float[] freqs)
        {
            int rate = 44100;
            float step = 0.085f;
            int total = Mathf.CeilToInt(rate * step * (freqs.Length + 1));
            float[] data = new float[total];
            for (int k = 0; k < freqs.Length; k++)
            {
                int start = Mathf.CeilToInt(rate * step * k);
                for (int i = 0; i < Mathf.CeilToInt(rate * step * 2.2f) && start + i < total; i++)
                {
                    float t = i / (float)rate;
                    float env = Mathf.Exp(-t * 9f);
                    float v = Mathf.Sin(2f * Mathf.PI * freqs[k] * t) * 0.32f
                              + Mathf.Sin(2f * Mathf.PI * freqs[k] * 2f * t) * 0.12f;
                    data[start + i] += v * env;
                }
            }
            AudioClip clip = AudioClip.Create(name, total, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }

        // ------------------------------------------------ música procedural

        /// <summary>Tema alegre de caja de música en Do mayor a 100 BPM, generado para repetirse sin costuras.</summary>
        static AudioClip MakeMusicLoop()
        {
            const int rate = 44100;
            const float bpm = 100f;
            const int bars = 8;

            float beat = 60f / bpm;
            float bar = beat * 4f;
            int n = Mathf.CeilToInt(rate * bar * bars);
            float[] data = new float[n];

            // Armonía: Do - Sol - Lam - Fa, dos compases cada uno.
            int[] bass = { 48, 48, 55, 55, 57, 57, 53, 53 };
            int[][] chords =
            {
                new[] { 60, 64, 67 }, new[] { 60, 64, 67 },
                new[] { 59, 62, 67 }, new[] { 59, 62, 67 },
                new[] { 57, 60, 64 }, new[] { 57, 60, 64 },
                new[] { 53, 57, 60 }, new[] { 53, 57, 60 },
            };

            for (int b = 0; b < bars; b++)
            {
                float t0 = b * bar;
                AddNote(data, rate, t0, bar * 0.97f, Midi(bass[b]), 0.15f, 0.9f, 0f);
                for (int c = 0; c < chords[b].Length; c++)
                    AddNote(data, rate, t0, bar * 0.97f, Midi(chords[b][c]), 0.042f, 0.9f, 0f);
            }

            // Melodía: una negra por tiempo.
            int[] melody =
            {
                72, 76, 79, 77,
                76, 74, 72, 72,
                74, 77, 81, 79,
                77, 76, 74, 72,
                79, 79, 76, 72,
                74, 77, 81, 84,
                83, 81, 79, 77,
                76, 74, 72, 72,
            };

            for (int i = 0; i < melody.Length; i++)
                AddNote(data, rate, i * beat, beat * 1.25f, Midi(melody[i]), 0.20f, 2.8f, 0.32f);

            // Suaviza el punto de unión del bucle.
            int fade = Mathf.CeilToInt(rate * 0.02f);
            for (int i = 0; i < fade; i++)
            {
                float k = i / (float)fade;
                data[i] *= k;
                data[n - 1 - i] *= k;
            }

            float peak = 0f;
            for (int i = 0; i < n; i++) peak = Mathf.Max(peak, Mathf.Abs(data[i]));
            if (peak > 0.85f)
            {
                float gain = 0.85f / peak;
                for (int i = 0; i < n; i++) data[i] *= gain;
            }

            AudioClip clip = AudioClip.Create("TemaPrincipal", n, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }

        static float Midi(int note)
        {
            return 440f * Mathf.Pow(2f, (note - 69) / 12f);
        }

        static void AddNote(float[] buffer, int rate, float startSec, float durSec, float freq, float amp, float decay, float bright)
        {
            int start = Mathf.CeilToInt(startSec * rate);
            int len = Mathf.CeilToInt(durSec * rate);
            for (int i = 0; i < len; i++)
            {
                int idx = start + i;
                if (idx >= buffer.Length) break;
                float t = i / (float)rate;
                float env = Mathf.Exp(-t * decay) * Mathf.Clamp01(t * 140f);
                float v = Mathf.Sin(2f * Mathf.PI * freq * t);
                if (bright > 0f)
                {
                    v += bright * Mathf.Sin(4f * Mathf.PI * freq * t);
                    v += bright * 0.45f * Mathf.Sin(6f * Mathf.PI * freq * t);
                }
                buffer[idx] += v * env * amp;
            }
        }
    }
}
