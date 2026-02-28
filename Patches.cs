using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace NoContainerDecay
{

    /// <summary>
    /// Patches Container.RemoveGear(GearItem, bool toInventory).
    /// Blocks destruction-driven removal of 0% items from containers.
    /// </summary>
    [HarmonyPatch(typeof(Container), nameof(Container.RemoveGear), new[] { typeof(GearItem), typeof(bool) })]
    internal static class PatchContainerRemoveGear
    {
        static bool Prefix(GearItem gi, bool toInventory)
        {
            if (gi == null) return true;
            if (toInventory) return true;

            float hp = -1f;
            try { hp = gi.m_CurrentHP; } catch { }

            if (hp <= 0f)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Patches GearManager.DestroyNextUpdate(GearItem, bool).
    /// Blocks deferred destruction when item is still in a container.
    /// </summary>
    [HarmonyPatch(typeof(GearManager), nameof(GearManager.DestroyNextUpdate))]
    internal static class PatchGearManagerDestroyNextUpdate
    {
        static bool Prefix(GearItem gearItem, bool value)
        {
            if (gearItem == null) return true;

            bool inContainer = false;
            try { inContainer = gearItem.m_InsideContainer; } catch { }

            bool parentIsContainer = false;
            try
            {
                Transform parent = gearItem.transform?.parent;
                if (parent != null)
                    parentIsContainer = parent.GetComponent<Container>() != null;
            }
            catch { }

            if (inContainer || parentIsContainer)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Patches GearManager.DestroyGearObject(GearItem) — last line of defense.
    /// </summary>
    [HarmonyPatch(typeof(GearManager), nameof(GearManager.DestroyGearObject), new[] { typeof(GearItem) })]
    internal static class PatchGearManagerDestroyGearObject
    {
        static bool Prefix(GearItem gi)
        {
            if (gi == null) return true;

            bool parentIsContainer = false;
            try
            {
                Transform parent = gi.transform?.parent;
                if (parent != null)
                    parentIsContainer = parent.GetComponent<Container>() != null;
            }
            catch { }

            if (parentIsContainer)
            {
                return false;
            }

            return true;
        }
    }
}