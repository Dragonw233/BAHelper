using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using BAHelper.Utility;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using ECommons.GameHelpers;

namespace BAHelper.Modules.General;

/// <summary>
/// 负责在水岛内给附近其他玩家绘制高亮提示。
/// </summary>
public sealed class PlayerHighlightService : IDisposable
{
    // 豪杰的记忆状态 ID；真实层数来源待确认，当前先用占位显示。
    private const uint HeroStatusId = 1742;
    // 文理魔盾状态 ID。
    private const uint ShellStatusId = 1643;
    private static Configuration Config => Plugin.Config;

    public PlayerHighlightService()
    {
        Svc.PluginInterface.UiBuilder.Draw += OnDraw;
    }

    public void Dispose()
    {
        Svc.PluginInterface.UiBuilder.Draw -= OnDraw;
    }

    // 仅在功能开启、玩家可用且不处于切换地图等状态时绘制。
    private static bool ShouldDraw =>
        Config.PlayerHighlightEnabled
        && Player.Available
        && Common.InHydatos
        && Svc.Objects.Length > 0
        && !(Svc.Condition[ConditionFlag.LoggingOut]
            || Svc.Condition[ConditionFlag.BetweenAreas]
            || Svc.Condition[ConditionFlag.BetweenAreas51]);

    // 每帧遍历当前可见对象，筛出玩家后绘制高亮。
    private static void OnDraw()
    {
        if (!ShouldDraw)
            return;

        var drawList = ImGui.GetBackgroundDrawList(ImGui.GetMainViewport());
        foreach (var player in Svc.Objects.OfType<IPlayerCharacter>())
        {
            DrawPlayer(drawList, player);
        }
    }

    // 给单个目标玩家绘制脚下圆环，并按配置显示名字。
    private static void DrawPlayer(ImDrawListPtr drawList, IPlayerCharacter player)
    {
        // 跳过自己、无效对象、不可选中对象和已死亡玩家，避免画出无意义标记。
        if (player.GameObjectId == Player.Object?.GameObjectId
            || !player.IsValid()
            || !player.IsTargetable
            || player.IsDead)
        {
            return;
        }

        // 超出设置范围的玩家不绘制，减少画面干扰。
        if (player.Position.Distance2D(Player.Position) > Config.PlayerHighlightDistance)
            return;

        // 只有命中至少一个已启用筛选条件的玩家才会被高亮。
        var matches = GetMatchedRules(player);
        if (matches.Count == 0)
            return;

        // 以配置半径为主，避免因为不同玩家碰撞箱导致圈大小忽大忽小。
        var radius = MathF.Max(Config.PlayerHighlightCircleRadius, 0.1f);
        for (var i = 0; i < matches.Count; i++)
        {
            var color = matches[i].Color;
            drawList.DrawRingWorld(player.Position, radius + i * 0.35f, 2f, color.SetAlpha(0.85f));
        }

        if (Config.PlayerHighlightShowName || Config.PlayerHighlightShowMatchedRules)
        {
            // 只有目标在屏幕内时才绘制名字/命中条件标签。
            Svc.GameGui.WorldToScreen(player.Position, out var screenPos, out var inView);
            if (inView)
            {
                var matchedRules = string.Join("/", matches.Select(match => match.Label));
                var label = (Config.PlayerHighlightShowName, Config.PlayerHighlightShowMatchedRules) switch
                {
                    (true, true) => $"{player.Name.TextValue} [{matchedRules}]",
                    (true, false) => player.Name.TextValue,
                    (false, true) => matchedRules,
                    _ => string.Empty
                };
                drawList.DrawTextTag(screenPos + new Vector2(0f, -34f), label, matches[0].Color, true, true, Color.TransBlack, true);
            }
        }
    }

    // 收集玩家命中的筛选项；多个命中项会画成多层不同颜色的圈。
    private static List<(string Label, uint Color)> GetMatchedRules(IPlayerCharacter player)
    {
        var matches = new List<(string Label, uint Color)>();
        if (Config.PlayerHighlightHeroEnabled && player.HasStatus(HeroStatusId))
            matches.Add(("豪杰x?", Config.PlayerHighlightHeroColor));
        if (Config.PlayerHighlightPerceptionEnabled && HasCarriedLogoAction(player, "探景"))
            matches.Add(("探景", Config.PlayerHighlightPerceptionColor));
        if (Config.PlayerHighlightIncenseEnabled && HasCarriedLogoAction(player, "激怒"))
            matches.Add(("激怒", Config.PlayerHighlightIncenseColor));
        if (Config.PlayerHighlightTankStanceEnabled && player.IsTankStanceActive())
            matches.Add(("盾姿", Config.PlayerHighlightTankStanceColor));
        if (Config.PlayerHighlightMissingShellEnabled && !player.HasStatus(ShellStatusId))
            matches.Add(("缺魔盾", Config.PlayerHighlightMissingShellColor));
        return matches;
    }

    // 探景、激怒这类文理技能通过玩家携带的两个文理动作名称判断，避免硬编码动作 RowId。
    private static bool HasCarriedLogoAction(IPlayerCharacter player, string keyword)
    {
        var logos = player.CarriedLogoActions();
        return LogoActionNameContains(logos.Item1, keyword) || LogoActionNameContains(logos.Item2, keyword);
    }

    private static bool LogoActionNameContains(uint logoActionId, string keyword)
    {
        return Common.LogoActionNames.TryGetValue(logoActionId, out var name)
            && name.Contains(keyword, StringComparison.Ordinal);
    }
}
