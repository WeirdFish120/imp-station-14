using Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Content.Shared.Mobs.Components;
using Robust.Shared.Map;
using Robust.Shared.Random;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Systems;

public sealed class ShuffleArtifactSystem : EntitySystem
{
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedTransformSystem _xform = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ShuffleArtifactComponent, ArtifactActivatedEvent>(OnActivated);
    }

    private void OnActivated(Entity<ShuffleArtifactComponent> ent, ref ArtifactActivatedEvent args)
    {
        var mobState = GetEntityQuery<MobStateComponent>();

        List<Entity<TransformComponent>> toShuffle = new();

        foreach (var target in _lookup.GetEntitiesInRange(ent, ent.Comp.Radius, LookupFlags.Dynamic | LookupFlags.Sundries))
        {
            if (!mobState.HasComponent(target))
                continue;

            var xform = Transform(target);

            toShuffle.Add((target, xform));
        }

        _random.Shuffle(toShuffle);

        while (toShuffle.Count > 1)
        {
            var ent1 = _random.PickAndTake(toShuffle);
            var ent2 = _random.PickAndTake(toShuffle);
            _xform.SwapPositions((ent1, ent1), (ent2, ent2));
        }
    }
}
