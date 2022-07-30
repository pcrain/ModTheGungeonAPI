﻿using System;
using System.Collections.Generic;
using System.Linq;

public static class GunExt
{

    private static GunAnimationSpriteCache SpriteCache = new GunAnimationSpriteCache();

    /// <summary>
    /// Sets the given item's name to the given string.
    /// </summary>
    /// <param name="item">The item to rename.</param>
    /// <param name="text">The new name for the item.</param>
    public static void SetName(this PickupObject item, string text)
    {
        ETGMod.Databases.Strings.Items.Set(item.encounterTrackable.journalData.PrimaryDisplayName, text);
    }

    /// <summary>
    /// Sets the given item's short description to the given string.
    /// </summary>
    /// <param name="item">The item to rename.</param>
    /// <param name="text">The new short description for the item.</param>
    public static void SetShortDescription(this PickupObject item, string text)
    {
        ETGMod.Databases.Strings.Items.Set(item.encounterTrackable.journalData.NotificationPanelDescription, text);
    }

    /// <summary>
    /// Sets the given item's long description to the given string.
    /// </summary>
    /// <param name="item">The item to rename.</param>
    /// <param name="text">The new long description for the item.</param>
    public static void SetLongDescription(this PickupObject item, string text)
    {
        ETGMod.Databases.Strings.Items.Set(item.encounterTrackable.journalData.AmmonomiconFullEntry, text);
    }

    /// <summary>
    /// Updates a gun's animation sprites if the animations have been setup, or sets them up otherwise.
    /// </summary>
    /// <param name="gun">The gun to process.</param>
    /// <param name="collection">The collection to get the sprites from. Defaults to WeaponCollection.</param>
    public static void UpdateAnimations(this Gun gun, tk2dSpriteCollectionData collection = null)
    {
        collection ??= ETGMod.Databases.Items.WeaponCollection;

        gun.idleAnimation = gun.UpdateAnimation("idle", collection);
        gun.dodgeAnimation = gun.UpdateAnimation("dodge", collection);
        gun.introAnimation = gun.UpdateAnimation("intro", collection, true);
        gun.emptyAnimation = gun.UpdateAnimation("empty", collection);
        gun.shootAnimation = gun.UpdateAnimation("fire", collection, true);
        gun.reloadAnimation = gun.UpdateAnimation("reload", collection, true);
        gun.chargeAnimation = gun.UpdateAnimation("charge", collection);
        gun.outOfAmmoAnimation = gun.UpdateAnimation("out_of_ammo", collection);
        gun.dischargeAnimation = gun.UpdateAnimation("discharge", collection);
        gun.finalShootAnimation = gun.UpdateAnimation("final_fire", collection, true);
        gun.emptyReloadAnimation = gun.UpdateAnimation("empty_reload", collection, true);
        gun.criticalFireAnimation = gun.UpdateAnimation("critical_fire", collection, true);
        gun.enemyPreFireAnimation = gun.UpdateAnimation("enemy_pre_fire", collection);
        gun.alternateShootAnimation = gun.UpdateAnimation("alternate_shoot", collection, true);
        gun.alternateReloadAnimation = gun.UpdateAnimation("alternate_reload", collection, true);
        gun.alternateIdleAnimation = gun.UpdateAnimation("alternate_idle", collection);
    }

    /// <summary>
    /// Updates a specific gun animation's sprites or sets it up if it doesn't exist.
    /// </summary>
    /// <param name="gun">The gun to update the animaiton for.</param>
    /// <param name="name">The name for the animation.</param>
    /// <param name="collection">The collection to get the sprites from. Defaults to WeaponCollection</param>
    /// <param name="returnToIdle">True if the animation should return to the idle animation after playing, false otherwise.</param>
    /// <returns>The full name for the created or updated animation.</returns>
    public static string UpdateAnimation(this Gun gun, string name, tk2dSpriteCollectionData collection = null, bool returnToIdle = false)
    {
        collection ??= ETGMod.Databases.Items.WeaponCollection;
        SpriteCache.UpdateCollection(collection);
        var frames = SpriteCache.TryGetAnimationFrames(collection.name, gun.name, name);

        if (frames == null)
            return null;

        string clipName = gun.name + "_" + name;
        tk2dSpriteAnimationClip clip = gun.spriteAnimator.Library.GetClipByName(clipName);
        if (clip == null)
        {
            clip = new tk2dSpriteAnimationClip();
            clip.name = clipName;
            clip.fps = 15;
            if (returnToIdle)
            {
                clip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            }
            Array.Resize(ref gun.spriteAnimator.Library.clips, gun.spriteAnimator.Library.clips.Length + 1);
            gun.spriteAnimator.Library.clips[gun.spriteAnimator.Library.clips.Length - 1] = clip;
        }

        clip.frames = frames;

        return clipName;
    }

    /// <summary>
    /// Sets the fps of all animations for the given gun to the given number.
    /// </summary>
    /// <param name="gun">The gun to process.</param>
    /// <param name="fps">The new fps for all of the animations.</param>
    public static void SetAnimationFPS(this Gun gun, int fps)
    {
        gun.SetAnimationFPS(gun.idleAnimation, fps);
        gun.SetAnimationFPS(gun.introAnimation, fps);
        gun.SetAnimationFPS(gun.emptyAnimation, fps);
        gun.SetAnimationFPS(gun.shootAnimation, fps);
        gun.SetAnimationFPS(gun.reloadAnimation, fps);
        gun.SetAnimationFPS(gun.chargeAnimation, fps);
        gun.SetAnimationFPS(gun.outOfAmmoAnimation, fps);
        gun.SetAnimationFPS(gun.dischargeAnimation, fps);
        gun.SetAnimationFPS(gun.finalShootAnimation, fps);
        gun.SetAnimationFPS(gun.emptyReloadAnimation, fps);
        gun.SetAnimationFPS(gun.criticalFireAnimation, fps);
        gun.SetAnimationFPS(gun.enemyPreFireAnimation, fps);
        gun.SetAnimationFPS(gun.alternateShootAnimation, fps);
        gun.SetAnimationFPS(gun.alternateReloadAnimation, fps);
    }
    /// <summary>
    /// Sets the fps of an animation with a given name for a given gun to the given number/
    /// </summary>
    /// <param name="gun">The gun to process.</param>
    /// <param name="name">The name of the animation to change.</param>
    /// <param name="fps">The new fps for the given animation.</param>
    public static void SetAnimationFPS(this Gun gun, string name, int fps)
    {
        if (string.IsNullOrEmpty(name) || gun == null || gun.spriteAnimator == null || gun.spriteAnimator.Library == null)
        {
            return;
        }

        tk2dSpriteAnimationClip clip = gun.spriteAnimator.Library.GetClipByName(name);
        if (clip == null)
        {
            return;
        }
        clip.fps = fps;
    }

    /// <summary>
    /// Adds a projectile from a gun with the given internal name to the given gun's list of possible projectiles.
    /// </summary>
    /// <param name="gun">The gun to add the projectile to.</param>
    /// <param name="other">The internal name of the gun to get the projectile from.</param>
    /// <returns>The added projectile.</returns>
    public static Projectile AddProjectileFrom(this Gun gun, string other)
    {
        return gun.AddProjectileFrom((Gun)ETGMod.Databases.Items[other]);
    }

    /// <summary>
    /// Adds a projectile to the given gun's list of possible projectiles from another given gun.
    /// </summary>
    /// <param name="gun">The gun to add the projectile to.</param>
    /// <param name="other">The gun to get the projectile from.</param>
    /// <returns>The added projectile.</returns>
    public static Projectile AddProjectileFrom(this Gun gun, Gun other)
    {
        if (other.DefaultModule.projectiles.Count == 0) return null;
        Projectile p = other.DefaultModule.projectiles[0];
        if (p == null) return null;
        return gun.AddProjectile(p);
    }

    /// <summary>
    /// Adds the given projectile to the given gun's list of possible projectiles.
    /// </summary>
    /// <param name="gun">The gun to add the projectile to.</param>
    /// <param name="projectile">The projectile to add.</param>
    /// <returns>The added projectile.</returns>
    public static Projectile AddProjectile(this Gun gun, Projectile projectile)
    {
        gun.DefaultModule.projectiles.Add(projectile);
        return projectile;
    }

    /// <summary>
    /// Adds a projectile module from a gun with the given internal name to the given gun's list of projectile modules.
    /// </summary>
    /// <param name="gun">The gun to add the projectile module to.</param>
    /// <param name="other">The internal name of the gun to get the projectile module from.</param>
    /// <returns>The added projectile module.</returns>
    public static ProjectileModule AddProjectileModuleFrom(this Gun gun, string other)
    {
        return gun.AddProjectileModuleFrom((Gun)ETGMod.Databases.Items[other]);
    }

    /// <summary>
    /// Adds a projectile module to the given gun's list of projectile modules from another given gun.
    /// </summary>
    /// <param name="gun">The gun to add the projectile module to.</param>
    /// <param name="other">The gun to get the projectile module from.</param>
    /// <returns>The added projectile module.</returns>
    public static ProjectileModule AddProjectileModuleFrom(this Gun gun, Gun other)
    {
        ProjectileModule module = other.DefaultModule;
        ProjectileModule clone = ProjectileModule.CreateClone(module, false);
        clone.chargeProjectiles = new(module.chargeProjectiles);
        clone.projectiles = new(module.projectiles);
        return gun.AddProjectileModule(clone);
    }

    /// <summary>
    /// Adds the given projectile module to the given gun's list of projectile modules.
    /// </summary>
    /// <param name="gun">The gun to add the projectile module to.</param>
    /// <param name="projectile">The projectile module to add.</param>
    /// <returns>The added projectile module.</returns>
    public static ProjectileModule AddProjectileModule(this Gun gun, ProjectileModule projectile)
    {
        gun.Volley.projectiles.Add(projectile);
        return projectile;
    }

    /// <summary>
    /// Sets the gun's default sprite to the gun's ammonomicon sprite, and if defaultSprite isn't null, sets the ammonomicon sprite to defaultSprite. If fps isn't 0, also sets the fps for all of the gun's animaiton to the given number.
    /// </summary>
    /// <param name="gun">The gun to do the setup for.</param>
    /// <param name="collection">The collection to get the default sprite from. Defaults to WeaponCollection.</param>
    /// <param name="defaultSprite">The name of the sprite to set the gun's ammonomicon sprite to. Defaults to null, which makes it not set the sprite to anything.</param>
    /// <param name="fps">The new fps for all of the gun's animations. Defaults to 0, which doesn't change the fps.</param>
    public static void SetupSprite(this Gun gun, tk2dSpriteCollectionData collection = null, string defaultSprite = null, int fps = 0)
    {
        collection ??= ETGMod.Databases.Items.WeaponCollection;

        if (defaultSprite != null)
        {
            gun.encounterTrackable.journalData.AmmonomiconSprite = defaultSprite;
        }

        gun.UpdateAnimations(collection);
        gun.GetSprite().SetSprite(collection, gun.DefaultSpriteID = collection.GetSpriteIdByName(gun.encounterTrackable.journalData.AmmonomiconSprite));

        if (fps != 0)
        {
            gun.SetAnimationFPS(fps);
        }
    }

}