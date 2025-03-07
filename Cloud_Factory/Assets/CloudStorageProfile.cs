using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.UIElements;
using TMPro;

public class CloudStorageProfile : MonoBehaviour
{
    public GameObject[] Profiles;
    SOWManager SOWManager;
    Guest GuestManager;
    RecordUIManager UIManager;
    private RLHReader mRLHReader;

    [SerializeField]
    Image[] iPortrait = new Image[20];

    public Button btNextBtn;           // 다음페이지 버튼
    public Button btPrevBtn;           // 이전페이지 버튼
    public Button btGiveBtn;           // 구름 제공 버튼

    // 가장 앞에 있는 프로필 오브젝트의 인덱스 정보
    [SerializeField]
    int frontProfileInfo;
    public GameObject mGetCloudContainer;

    // 화면상에 나오고 있는 손님의 손님번호
    int frontGuestIndex;

    // 구름을 제공받을 수 있는 손님들의 손님번호 리스트
    [SerializeField]
    int[]   UsingGuestNumList;
    int     UsingGuestIndex;

    // 구름을 제공받을 수 있는 손님들의 리스트의 길이
    int numOfUsingGuestList;

    public Toggle   plusToggle;
    public Toggle   minusToggle;
    private int sat = 0;

    [SerializeField]
    Sprite emptyProfileImage;

    void Awake()
    {
 
    }

    private void Start()
    {
        // 데이터 불러오기
        SOWManager = GameObject.Find("SOWManager").GetComponent<SOWManager>();
        GuestManager = GameObject.Find("GuestManager").GetComponent<Guest>();
        UIManager = GameObject.Find("UIManager").GetComponent<RecordUIManager>();
        mRLHReader = GameObject.Find("ProfileGroup").GetComponent<RLHReader>();
        mRLHReader = RLHReader.GetInstance();

        // TODO :  구름 보관함에서 제공받을 수 있는 뭉티를 고정한다. (계절에 따라 고정)lll
        int Season = GameObject.Find("Season Date Calc").GetComponent<SeasonDateCalc>().mSeason;
        UsingGuestNumList = GetSeansonGuestList(Season);
        numOfUsingGuestList = UsingGuestNumList.Length;

        frontProfileInfo = 0;
        UsingGuestIndex = 0;

        Profiles = new GameObject[3];
        Profiles[0] = GameObject.Find("I_ProfileBG1");
        Profiles[1] = GameObject.Find("I_ProfileBG2");
        Profiles[2] = GameObject.Find("I_ProfileBG3");

        btGiveBtn = GameObject.Find("B_CloudGIve").GetComponent<Button>();



        if (numOfUsingGuestList != 0)
        {
            frontGuestIndex = UsingGuestNumList[UsingGuestIndex];
            updateProfileList();
        }
    }

    public void GetNextProfileBtn()
    {
        if (numOfUsingGuestList == 0)
            return;

        if (TutorialManager.GetInstance().isTutorial) return;
        Invoke("GetNextProfile", 0.18f);
    }
    public void GetPrevProfileBtn()
    {
        if (numOfUsingGuestList == 0)
            return;

        if (TutorialManager.GetInstance().isTutorial) return;
        Invoke("GetPrevProfile", 0.18f);
    }

    private void GetNextProfile()
    {
        // 맨앞에 나온 손님의 인덱스가 
        if (UsingGuestIndex >= numOfUsingGuestList - 1)
        {
            UsingGuestIndex = numOfUsingGuestList - 1;
        }
        
        // 이전 프로필을 불러온다.
        frontProfileInfo = (frontProfileInfo + 1) % 3;
        UsingGuestIndex++;

        if (UsingGuestIndex > numOfUsingGuestList - 1)
        {
            UsingGuestIndex = 0;
        }
        frontGuestIndex = UsingGuestNumList[UsingGuestIndex];
        updateProfileList();
    }

    private void GetPrevProfile()
    {
        // 다음 프로필을 불러온다.       
        if (UsingGuestIndex <= 0)
        {
            UsingGuestIndex = 0;
        }
        
        // 다음 프로필을 불러온다.
        frontProfileInfo = (frontProfileInfo - 1 + 3) % 3;
        UsingGuestIndex--;

        // 다음 프로필을 불러온다.       
        if (UsingGuestIndex < 0)
        {
            UsingGuestIndex = numOfUsingGuestList - 1;
        }
        frontGuestIndex = UsingGuestNumList[UsingGuestIndex];
        updateProfileList();
    }

    private void Update()
    {
        UpdateBtGive();
    }

    void UpdateBtGive()
    {
        // TODO : 실시간으로 손님이 착석하면 사용할 수 있는 버튼을 활성화해준다.
        // 1. 현재 보고 있는 손님에 대한 정보를 받아온다.
        // 2. 확인한다.
        // 3. 갱신한다.   

        GameObject Profile = Profiles[frontProfileInfo];

        // Button
        btGiveBtn = Profile.transform.GetChild(1).GetComponent<Button>();

        List<int> UsingList = SOWManager.mUsingGuestList;
        bool test = false;
        for (int i = 0; i < UsingList.Count; i++)
        {
            if (UsingList[i] == frontGuestIndex)
            {
                // isSit 상태를 확인
                GuestObject obj = SOWManager.mUsingGuestObjectList[i].GetComponent<GuestObject>();
                if (obj != null && obj.isSit)
                {
                    if (obj.mGuestNum != frontGuestIndex)
                    {
                        continue;
                    }
                    test = true;
                    break;
                }
            }
        }
        if (test)
        {
            btGiveBtn.interactable = true;
        }
        else
        {
            btGiveBtn.interactable = false;
        }
    }

    IEnumerator initProfileList()
    {
        updateProfileList();
        yield return new WaitForEndOfFrame();
    }

    void updateProfileList()
    {
        GameObject Profile = Profiles[frontProfileInfo];
        {
            // Button
            btGiveBtn = Profile.transform.GetChild(1).GetComponent<Button>();

            // DEMO 버전 추가사항
            // 해당 손님이 날씨의 공간에 존재하지 않는다면 제공버튼이 비활성화 된다.
            {
                List<int> UsingList = SOWManager.mUsingGuestList;
                bool test = false;
                for (int i = 0; i < UsingList.Count; i++)
                {
                    if (UsingList[i] == frontGuestIndex)
                    {
                        // isSit 상태를 확인
                        GuestObject obj = SOWManager.mUsingGuestObjectList[i].GetComponent<GuestObject>();
                        if (obj != null && obj.isSit)
                        {
                            if (obj.mGuestNum != frontGuestIndex)
                            {
                                continue;
                            }
                            test = true;
                            break;
                        }
                    }
                }
                if (test)
                {
                    btGiveBtn.interactable = true;
                }
                else
                {
                    btGiveBtn.interactable = false;
                }
            }
        }

        // Image
        Image iProfile = Profile.transform.GetChild(0).GetComponent<Image>();

        // 나이 이름 직업
        TMP_Text tName = Profile.transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<TMP_Text>();
        Text tAge = Profile.transform.GetChild(3).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        Text tJob = Profile.transform.GetChild(4).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();

        // 만족도, 한 줄 요약
        Text tSat = Profile.transform.GetChild(5).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();

        // 한 줄 요약
        Text tSentence = Profile.transform.GetChild(6).GetComponent<Text>();

        if (frontGuestIndex != -1)
        {

            // 뭉티 정보를 가져온다.
            GuestInfos info = GuestManager.GetComponent<Guest>().mGuestInfo[frontGuestIndex];

            // 가져온 정보값을 이용하여 채운다.
            iProfile.sprite = UIManager.sBasicProfile[frontGuestIndex];

            if (LanguageManager.GetInstance().GetCurrentLanguage() == "English")
            {
                tSentence.text = "Summary: " + mRLHReader.LoadSummaryInfo(frontGuestIndex);
                tName.text = info.mNameEN;
                tJob.text = info.mJobEN;
            }
            else
            {
                tSentence.text = "한 줄 요약: " + mRLHReader.LoadSummaryInfo(frontGuestIndex);
                tName.text = info.mName;
                tJob.text = info.mJob;
            }
            tAge.text = "" + info.mAge;
            tSat.text = "" + info.mSatatisfaction;
        }
        else
        {
            iProfile.sprite = emptyProfileImage;

            if (LanguageManager.GetInstance().GetCurrentLanguage() == "English")
            {
                tSentence.text = "Summary: ";
            }
            else
            {
                tSentence.text = "한 줄 요약: ";
            }
            tName.text = "";
            tJob.text = "";
            tAge.text = "";
            tSat.text = "";
        }
    }

    void updateButton()
    {

    }

    public void GiveCloud()
    {
        TutorialManager mTutorialManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
        if (mTutorialManager.isFinishedTutorial[7] == false) mTutorialManager.isFinishedTutorial[7] = true;

        SOWManager = GameObject.Find("SOWManager").GetComponent<SOWManager>();

        // 구름 제공하는 메소드 호출
        StoragedCloudData storagedCloudData
            = mGetCloudContainer.GetComponent<CloudContainer>().mSelecedCloud;

        //구름 제공 예외처리
        if (storagedCloudData == null || numOfUsingGuestList == 0)
        {
            return;
        }

        List<int> UsingList = SOWManager.mUsingGuestList;
        bool test = false;
        for (int i = 0; i < UsingList.Count; i++)
        {
            if (UsingList[i] == frontGuestIndex)
            {
                test = true;
                break;
            }
        }
        if (!test)
        {
            return;
        }
        
        // IsUsing 상태 변경 
        int guestNum = frontGuestIndex;
        GuestManager.mGuestInfo[guestNum].isUsing = true;

        //구름인벤토리에서 사용된 구름 삭제
        mGetCloudContainer.GetComponent<CloudContainer>().deleteSelectedCloud();

		// 리스트에서 사용받은 손님 제거하기
		SOWManager sow = GameObject.Find("SOWManager").GetComponent<SOWManager>();
        int count = sow.mUsingGuestList.Count;
		for (int i = count - 1; i >= 0; i--)
        {
            if (sow.mUsingGuestList[i] == guestNum)
            {
                sow.mUsingGuestList.RemoveAt(i);
				sow.mUsingGuestObjectList.RemoveAt(i);
			}
        }

        // QA용
        {
            if(plusToggle.isOn == true)
            {
                sat = 1;
            }
            else if(minusToggle.isOn == true)
            {
                sat = -1;
            }
            else
            {
                sat = 0;
            }
        }

		SOWManager.SetCloudData(guestNum, storagedCloudData, sat);
		SceneManager.LoadScene("Space Of Weather");
    }

    int[] GetSeansonGuestList(int Season)
    {
        int[] list = new int[5];

        if (Season == 1)
        {
            list = new int[] { 0,1,3,4,6 };
        }
        else if (Season == 2)
        {
            list = new int[] { 12,13,14,18,19 };
        }
        else if(Season == 3)
        {
            list = new int[] { 2,8,9,10,16};
        }
        else if(Season == 4)
        {
            list = new int[] { 5,7,11,15,17 };
        }

        List<int> resultLIst = new List<int>();

        foreach(int i in list)
        {
            // 날씨의 공간에 존재하는지 확인한다.
             bool exist = false;
            foreach (int j in SOWManager.mUsingGuestList)
            {
                if(i == j)
                {
                    exist = true;
                    break;
                }    
            }

            if (exist == true)
            {
                if (GuestManager.mGuestInfo[i].isDisSat == false)
                    resultLIst.Add(i);
                continue;
            }

            foreach (int j in SOWManager.mWaitGuestList)
            {
                if (i == j)
                {
                    exist = true;
                    break;
                }
            }

            if (exist == true)
            {
                if (GuestManager.mGuestInfo[i].isDisSat == false)
                    resultLIst.Add(i);
                continue;
            }
        }

        while(resultLIst.Count < 3)
        {
            resultLIst.Add(-1);
        }

        return resultLIst.ToArray();
    }


    // TEST
    public void Plus()
    {
        minusToggle.isOn = false;
    }

    public void Minus()
    {
        plusToggle.isOn = false;
    }
}