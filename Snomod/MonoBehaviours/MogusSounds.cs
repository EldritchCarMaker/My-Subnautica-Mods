using System.IO;
using System.Security.Policy;
using Nautilus.FMod;
using Nautilus.Utility;
using Snomod.Prefabs;
using UnityEngine;

namespace Snomod.MonoBehaviours;

internal class MogusSounds : MonoBehaviour, IOnTakeDamage
{
    public static readonly AssetBundle soundsBundle = AssetBundle.LoadFromFile(Path.Combine(Amogus.assetsPath, "amgomugussounds"));
    private static readonly FMODAsset hurtSound = GetFmodAsset("Amogus_EmergencyMeeting");
    public static readonly FMODAsset deathSound = GetFmodAsset("Amogus_Death");
    private static readonly FMODAsset passiveSound = GetFmodAsset("Amogus_Impostor");
    private static bool hasRegisteredSounds = false;

    private float timeLastHurt = 0;
    private float timeLastPassiveSound = 0;
    private void Awake()
    {
        if (hasRegisteredSounds)
            return;
        var crawl = GetComponent<CaveCrawler>();
        crawl.jumpSound = GetFmodAsset("event:/creature/crawler/jump");
        crawl.walkingSound.SetAsset(GetFmodAsset("event:/creature/crawler/idle"));

        var builder = new FModSoundBuilder(new AssetBundleSoundSource(soundsBundle));

        builder.CreateNewEvent("Amogus_Impostor", AudioUtils.BusPaths.UnderwaterCreatures)
            .SetMode3D(7, 13, false)
            .SetSound("Impostor (quiet)")
            .Register();

        builder.CreateNewEvent("Amogus_EmergencyMeeting", AudioUtils.BusPaths.UnderwaterCreatures)
            .SetMode3D(7, 10, false)
            .SetSound("Emergency Meeting (quiet)")
            .Register();

        builder.CreateNewEvent("Amogus_Death", AudioUtils.BusPaths.UnderwaterCreatures)
            .SetMode3D(10, 20, false)
            .SetSound("Death")
            .Register();

        hasRegisteredSounds = true;
    }

    private void FixedUpdate()//Because we use randomness to decide if we want to play the sound or not, we use fixed update to ensure that the sound chances are consistent across framerates. Update would cause the sound to trigger quicker the higher your framerate, and less often the lower.
    {
        if (timeLastPassiveSound + 15 > Time.time)
            return;

        if(Random.value < 0.01f)//Should average out to trigger after a few seconds on average
        {
            timeLastPassiveSound = Time.time;
            FMODUWE.PlayOneShot(passiveSound, transform.position);
        }
    }

    public void OnKill()//It's a unity BroadcastMessage, not an interface method. Hate it.
    {
        FMODUWE.PlayOneShot(deathSound, transform.position);
    }
    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if(timeLastHurt + 1.5f > Time.time) //Don't want the sound to play multiple times in quick succession, so we check if it's been at least 0.5 seconds since the last time it played.
            return;
        timeLastHurt = Time.time;
        FMODUWE.PlayOneShot(hurtSound, transform.position);
    }
    public static FMODAsset GetFmodAsset(string audioPath)
    {
        FMODAsset asset = ScriptableObject.CreateInstance<FMODAsset>();
        asset.path = audioPath;
        return asset;
    }
}
