using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    public GameObject weaponObject;
    public ParticleSystem smoke;
    public Animator weaponAnimator;

    public Transform bulletParentWeapon, target;
    public List<GameObject> bullets;

    public void ReloadBullets(List<GameObject> bulletsToReload)
    {
        StartCoroutine(ReloadBulletsCoroutine(bulletsToReload));
    }

    private IEnumerator ReloadBulletsCoroutine(List<GameObject> bulletsToReload)
    {
        weaponAnimator.SetTrigger("ReloadState");
        yield return new WaitForSeconds(1f);
        foreach (var bullet in bulletsToReload)
        {
            ReloadOneBullet(bullet);
            yield return new WaitForSeconds(1.5f); // Delay between each reload
        }
    }

    private void ReloadOneBullet(GameObject bullet)
    {
      
            // Play revolving sound and show chamber rolling animation
            bullet.transform.DOLocalMoveY(2f, 0.3f).OnUpdate(() =>
            {
                bullet.transform.Rotate(Vector3.up, 10);
            }).OnComplete(() =>
            {
               
                bullet.transform.DOMove(bulletParentWeapon.position, 1).OnComplete(() =>
                {
                    weaponAnimator.SetTrigger("ReloadOne");
                    bullet.transform.parent = bulletParentWeapon;
                    bullets.Add(bullet);
                });
            });
       
    }

    public void Shoot(UnityAction bulletHitAction)
    {
        transform.DOLocalRotate(Vector3.zero, 1f).SetEase(Ease.OutSine);
        transform.DOLocalMove(Vector3.zero, 1f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            // Play revolving sound and show chamber rolling animation
            GameObject currentBullet = bullets[0];
            bullets.RemoveAt(0);
            currentBullet.transform.DOMove(target.position, 0.2f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                bulletHitAction.Invoke();
            });
        });
    }
}
