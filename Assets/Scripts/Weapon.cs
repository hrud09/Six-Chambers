using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
public class Weapon : MonoBehaviour
{

    public GameObject weaponObject;
    public ParticleSystem smoke;
    public Transform restingParent, reloadingParent, shootingParent;
    public Transform bulletHoldingParent;
    public Transform target;
    public UnityEvent bulletHitAction;
    public List<GameObject> bullets;
    public void Reload(GameObject bullet)
    {
        transform.parent = reloadingParent;
        transform.DOLocalRotate(Vector3.zero, 1f).SetEase(Ease.OutSine);
        transform.DOLocalMove(Vector3.zero, 1f).SetEase(Ease.OutSine).OnComplete(() => {

            //Play revolving sound and show chamber rolling animation
            //Fly bullet to the chamber from the chambers
            bullet.transform.DOLocalMoveY(2f, 0.3f).OnUpdate(() => {

                bullet.transform.Rotate(Vector3.up, 10);
            
            }).OnComplete(() => {

                Vector3 midPos = new Vector3(((bullet.transform.position - transform.position)/2).x, transform.position.y, transform.position.z);
                List<Vector3> pathPoints = new List<Vector3>();
                pathPoints.Add(midPos);
                pathPoints.Add(bulletHoldingParent.position);
                bullet.transform.DOPath(pathPoints.ToArray(), 1).OnComplete(() => {

                    bullet.transform.parent = bulletHoldingParent;
                    bullets.Add(bullet);
                });
            });
        });
    }


    public void Shoot()
    {
        transform.parent = shootingParent;
        transform.DOLocalRotate(Vector3.zero, 1f).SetEase(Ease.OutSine);
        transform.DOLocalMove(Vector3.zero, 1f).SetEase(Ease.OutSine).OnComplete(() => {

            //Play revolving sound and show chamber rolling animation
            //Fly bullet to the chamber from the chambers

             GameObject currentBullet = bullets[0];
             currentBullet.transform.DOMove(target.position, 0.2f).SetEase(Ease.InCubic).OnComplete(() => {

                 bulletHitAction.Invoke();
             
             });
        });
    }


}
