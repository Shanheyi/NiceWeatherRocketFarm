using UnityEngine;
using UnityEngine.UI;

public class HotbarSelector : MonoBehaviour
{
    public static HotbarSelector Instance;

    public int totalSlots = 7;
    public WeaponController weaponController;
    public SeedSelector seedSelector;
    public Image[] slotBackgrounds;
    public Image[] slotIcons;
    public Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.7f);
    public Color selectedColor = new Color(1f, 0.9f, 0.4f, 0.9f);
    public Color lockedColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);

    private int currentIndex = 0;

    public int CurrentIndex => currentIndex;
    public bool IsWeaponSelected => currentIndex >= 3;
    public bool IsSeedSelected => currentIndex == 0 || currentIndex == 1;
    public bool IsEmptySlot => currentIndex == 2;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentIndex = 0;
        if (seedSelector != null)
        {
            seedSelector.SelectSeed(0);
        }
        GameManager.OnWeaponUnlocked += OnWeaponUnlockedHandler;
        GameManager.OnEggplantSeedChanged += OnEggplantSeedChangedHandler;
        RefreshAllSlots();
    }

    private void OnDestroy()
    {
        GameManager.OnWeaponUnlocked -= OnWeaponUnlockedHandler;
        GameManager.OnEggplantSeedChanged -= OnEggplantSeedChangedHandler;
    }

    private void Update()
    {
        for (int i = 0; i < totalSlots; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectSlot(i);
            }
        }
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= totalSlots)
        {
            return;
        }

        if (index >= 3)
        {
            int weaponIndex = index - 3;
            if (!GameManager.Instance.IsWeaponUnlocked(weaponController.allWeapons[weaponIndex].weaponName))
            {
                ToastManager.Instance.Show("武器未解锁");
                return;
            }
        }

        currentIndex = index;

        if (index == 0 || index == 1)
        {
            seedSelector.SelectSeed(index);
        }
        else if (index >= 3)
        {
            weaponController.SelectWeapon(index - 3);
        }

        RefreshAllSlots();
    }

    public void RefreshAllSlots()
    {
        if (GameManager.Instance != null && slotIcons != null && slotIcons.Length > 1 && slotIcons[1] != null)
        {
            bool hasEggplantSeed = GameManager.Instance.eggplantSeedCount > 0;
            slotIcons[1].enabled = hasEggplantSeed;
        }

        for (int i = 0; i < totalSlots; i++)
        {
            if (i >= 3)
            {
                bool isUnlocked = IsWeaponUnlocked(i - 3);
                if (!isUnlocked)
                {
                    slotBackgrounds[i].color = lockedColor;
                    slotIcons[i].enabled = false;
                }
                else
                {
                    if (i == 3)
                    {
                        slotIcons[i].sprite = weaponController.allWeapons[0].unlockedIcon;
                    }
                    slotIcons[i].enabled = true;
                    slotBackgrounds[i].color = i == currentIndex ? selectedColor : normalColor;
                }
            }
            else
            {
                slotBackgrounds[i].color = i == currentIndex ? selectedColor : normalColor;
            }
        }
    }

    private bool IsWeaponUnlocked(int weaponIndex)
    {
        if (weaponIndex == 0)
        {
            return GameManager.Instance.IsWeaponUnlocked("Pistol");
        }

        WeaponData weapon = weaponController.allWeapons[weaponIndex];
        return GameManager.Instance.IsWeaponUnlocked(weapon.weaponName);
    }

    private void OnWeaponUnlockedHandler(string weaponName)
    {
        RefreshAllSlots();
    }

    private void OnEggplantSeedChangedHandler(int count)
    {
        RefreshAllSlots();
    }
}
