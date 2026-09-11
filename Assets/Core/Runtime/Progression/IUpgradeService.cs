using System;

namespace hp55games.Mobile.Core.Progression
{
    /// <summary>
    /// Contract for unlocking UpgradeNodeData nodes. CanUnlock and TryUnlock are deliberately
    /// separate: CanUnlock lets UI query unlock-eligibility (cost + prerequisites) every frame
    /// for button enable/disable state without side effects, TryUnlock is the one call that
    /// actually spends currency and mutates unlock state. Pure interface, no base class — not
    /// registered by ServiceRegistry.InstallDefaults(); a concrete project implements and
    /// registers its own.
    /// </summary>
    public interface IUpgradeService
    {
        bool CanUnlock(UpgradeNodeData node);

        bool TryUnlock(UpgradeNodeData node);

        bool IsUnlocked(UpgradeNodeData node);

        event Action<UpgradeNodeData> OnNodeUnlocked;
    }
}
