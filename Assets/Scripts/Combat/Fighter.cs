using UnityEngine;
using RPG.Attributes;
using RPG.Core;
using RPG.Control;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float WeaponRange = 5f;
        [SerializeField] float timeBetweenAttacks = 1f;

        [SerializeField] float weaponDamage = 8f;
        Transform Target;
        float timeSinceLastAttack = Mathf.Infinity;

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            AnimatorStateInfo stateInfo = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            if (stateInfo.normalizedTime >= 1.0f && stateInfo.IsName("Attack")) GetComponent<Animator>().SetTrigger("stopAttack");

        }

        public void Attack(CombatTarget combatTarget)
        {
            Target = combatTarget.transform;

            if (Target != null && !IsInRange(Target)) Debug.Log("not attacking target");
            else
            {
                Debug.Log("attacking target");
                AttackBehaviour();
            }
        }
        public void Cancel()
        {
            GetComponent<Animator>().SetTrigger("stopAttack");
            Target = null;
        }


        private bool IsInRange(Transform target)
        {
            return Vector3.Distance(transform.position, target.position) < WeaponRange;
        }


        private void AttackBehaviour()
        {
            // transform.LookAt(Target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                // This will trigger the Hit() event.
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        //Animation event
        void Hit()
        {
            if (Target == null) return;
            Health healthComponent = Target.GetComponent<Health>();
            healthComponent.TakeDamage(weaponDamage);
        }

        private void TriggerAttack()
        {
            GetComponent<ActionScheduler>().StartAction(this);
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }
    }
}
