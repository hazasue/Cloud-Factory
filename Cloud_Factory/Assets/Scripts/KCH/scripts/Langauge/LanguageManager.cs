using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    private static LanguageManager instance;
    
    [SerializeField]
    private bool isKorean;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if(this != instance)
            {
                Destroy(this.gameObject);
            }
        }
        instance = this;
        isKorean = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static LanguageManager GetInstance()
    {
        return instance;
    }

    public void SetEnglish()
    {
        isKorean = false;
    }

    public void SetKorean()
    {
        isKorean = true;
    }

    public string GetCurrentLanguage()
    {
        if (isKorean == false) return "English";
        return "Korean";
    }

    public bool GetIsKorean()
    {
        return isKorean;
    }
    public void SetLanguageData(bool _isKorean)
    {
        isKorean = _isKorean;
    }
}
