using LuckyBlock.BlockEvents;
using LuckyBlock.BlockEvents.SpawnEvents;
using SMLHelper.V2.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace LuckyBlock.MonoBehaviours
{
    public class LuckyBlockMono : HandTarget, IHandTarget
    {
        public static LuckyBlockEvent[] luckyBlockEvents;//set in the main patch method

        private static readonly FMODAsset clickSound = GetFmodAsset("event:/loot/hit_breakable");//Comment out if using .dll in unity editor! Unity does NOT like ScriptableObject.CreateInstance in field initializers
        private static LuckyBlockEvent previousEvent;
        private static List<LuckyBlockEvent> previousEvents = new();
        private const int HitsToBreak = 5;

        private int hitCount = 0;
        [SerializeField]
        private LuckyBlockRarity _blockRarity;
        [SerializeField]
        private CubeOpenEffects _cubeEffects;

        //convenience getters.
        internal float Depth => -transform.position.y;
        internal string Biome => WaterBiomeManager.main.GetBiome(transform.position);
        internal Vector3 Position => transform.position;
        internal BlockType Rarity => _blockRarity.Type;

        public void OnHandClick(GUIHand hand)
        {
            if (hitCount < HitsToBreak)
            {
                hitCount++;
                if (hitCount == HitsToBreak) BreakOpen();
                Utils.PlayFMODAsset(clickSound, transform);
            }
        }

        public void OnHandHover(GUIHand hand)
        {
            if (hitCount < HitsToBreak)
            {
                HandReticle.main.SetInteractText($"Open {Rarity} lucky block");
                HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
            }
        }

        public void BreakOpen()
        {
            _cubeEffects.PlayOpenEffects();

            var effect = PickEvent();
            if(Main.DebugLogs) ErrorMessage.AddMessage($"Found event {effect}");
            if (effect is IDelayedEffect delayedEffect)
            {
                if (Main.DebugLogs) ErrorMessage.AddMessage($"found delayed event {effect} with message `{effect.Message}`, and delayed message `{delayedEffect.Message}`");
                if (!string.IsNullOrEmpty(delayedEffect.Message)) ErrorMessage.AddMessage(delayedEffect.Message);
                CoroutineHost.StartCoroutine(PlayDelayedEvent(effect, delayedEffect.Delay, this));
            }
            else
            {
                if (!string.IsNullOrEmpty(effect.Message)) ErrorMessage.AddMessage(effect.Message);

                try
                {
                    effect.TriggerEffect(this);
                }
                catch (Exception e)
                {
                    ErrorMessage.AddMessage("Whoops! Error occurred when trying to run the event!");
                    QModManager.Utility.Logger.Log(QModManager.Utility.Logger.Level.Error, $"Error when running event type {effect.GetType()}", e);
                }
            }
            previousEvent = effect;
            if(Main.Config.testVersion)
            {
                previousEvents.Add(effect);
                if (previousEvents.Count >= luckyBlockEvents.Length)
                    previousEvents.Clear();
            }
        }

        public static IEnumerator PlayDelayedEvent(LuckyBlockEvent effect, float delay, LuckyBlockMono block)
        {
            if (Main.DebugLogs) ErrorMessage.AddMessage($"Delayed coroutine waiting for {delay} seconds");
            yield return new WaitForSeconds(delay);
            if (!string.IsNullOrEmpty(effect.Message)) ErrorMessage.AddMessage(effect.Message);
            effect.TriggerEffect(block);
            if (Main.DebugLogs) ErrorMessage.AddMessage("Triggered delayed effect");
        }

        public LuckyBlockEvent PickEvent()
        {
            IEnumerable<LuckyBlockEvent> validEvents;
            if(!Main.Config.testVersion)
                validEvents = luckyBlockEvents.Where(effect => effect.CanTrigger(this));
            else
                validEvents = luckyBlockEvents.Where(effect => !previousEvents.Contains(effect) && effect.CanTrigger(this));

            var chosen = Main.WeightedRandom<LuckyBlockEvent>((@event) => @event == previousEvent ? @event.GetWeight(this) * 0.25f : @event.GetWeight(this), validEvents);

            if(chosen == null)
            {
                if(Main.Config.testVersion && previousEvents.Count > 4)
                {
                    previousEvents.Clear();
                    return PickEvent();
                }
                ErrorMessage.AddMessage("A problem has occured in the mod. Could not find any valid events to run");
            }
            return chosen;
        }

        public static FMODAsset GetFmodAsset(string audioPath)
        {
            FMODAsset asset = ScriptableObject.CreateInstance<FMODAsset>();
            asset.path = audioPath;
            return asset;
        }

        public void OnEnable()
        {
            LuckyBlockSpawner.blocks.Add(this);
        }

        public void OnDisable()
        {
            LuckyBlockSpawner.blocks.Remove(this);
        }

    }
}
