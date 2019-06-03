public interface IHealable
{
	void OnHeal(float healAmount);
	void OnTakeDamage(float damage);
	void OnHealthDeplete();
}
