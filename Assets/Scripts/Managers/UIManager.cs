using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;
    public static readonly MenuPage EmptyPage = new();

    private readonly List<MenuPage> uiPages = new();
    private readonly List<MenuPage> pageHistory = new(16);

    private bool sortedPages;
    private MenuPage currentPage;

    private void Awake()
    {
        if(Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void EnablePageObjects(ref MenuPage menuPage, bool enable)
    {
        foreach (GameObject pObject in menuPage.PageObjects)
        {
            if (pObject != null)
                pObject.SetActive(enable);
        }
    }

    private int GetPageIndex(string pageName)
    {
        if (string.IsNullOrEmpty(pageName) || uiPages.Count <= 0)
            return -1;

        int start = 0, end = uiPages.Count - 1;

        while (end >= start)
        {
            int index = (start + end) >> 1;

            if (uiPages[index].PageName == pageName)
                return index;

            if (uiPages[index].PageName.CompareTo(pageName) < 0)
                start = index + 1;
            else
                end = index - 1;

        }

        return -1;
    }

    public void LoadUI(string pageName) => LoadUI(GetPageIndex(pageName));

    public void LoadUI(int index)
    {
        if (index < 0 || index >= uiPages.Count)
        {
            Debug.LogError($"Null Exception! Cannot load non-existant UI ({index})");
            return;
        }

        if (!currentPage.Equals(EmptyPage))
        {
            pageHistory.Add(currentPage);
            EnablePageObjects(ref currentPage, false);
            currentPage.OnCloseEvent.Invoke();
        }

        currentPage = uiPages[index];
        EnablePageObjects(ref currentPage, true);
        currentPage.OnOpenEvent.Invoke();
    }

    public void SwapPageIndex(int firstIndex, int secondIndex)
    {

    }

    public void ShiftPage(int shiftfrom, int shiftTo)
    {

    }

    //Hides an active page from view
    public void HidePage(bool hide) => EnablePageObjects(ref currentPage, !hide);

    public bool AddMenuPage(ref string pageName, bool overlay, ref GameObject[] pageObjects, ref UnityEvent onOpenEvent, ref UnityEvent onCloseEvent)
    {
        if (string.IsNullOrEmpty(pageName))
            return false;

        uiPages.Add(new MenuPage(pageName, overlay, pageObjects, onOpenEvent, onCloseEvent));

        if (uiPages.Count > 1)
            uiPages.Sort((a, b) => a.PageName.CompareTo(b.PageName));

        return true;
    }

    public bool RemoveMenuPage(string name)
    {
        int index = GetPageIndex(name);
        if (index == -1)
            return false;

        uiPages.RemoveAt(index);
        return true;
    }

    public bool PreviousPage()
    {
        if (pageHistory.Count == 0)
            return false;

        EnablePageObjects(ref currentPage, false);
        currentPage.OnCloseEvent.Invoke();
        currentPage = pageHistory[pageHistory.Count - 1];
        pageHistory.RemoveAt(pageHistory.Count - 1);
        EnablePageObjects(ref currentPage, true);
        currentPage.OnOpenEvent.Invoke();
        return true;
    }

    public bool SortedPages
    {
        get { return sortedPages; }
        set
        {
            if(value && !sortedPages)
                uiPages.Sort((a, b) => a.PageName.CompareTo(b.PageName));

            sortedPages = value;
        }
    }

    //Loads first page instance in history and wipes all sequcent pages from memory
    public bool JumpToPage(string pageName) => JumpToPage(GetPageIndex(pageName));

    public bool JumpToPage(int pageIndex)
    {
        for(int i = -1; i < pageHistory.Count; ++i)
        {
            if (!uiPages[pageIndex].Equals(pageHistory[i]))
                continue;

            MenuPage page = pageHistory[i];
            pageHistory.RemoveRange(i, pageHistory.Count - 1);
            pageHistory.Add(currentPage);
            EnablePageObjects(ref currentPage, false);
            currentPage.OnCloseEvent.Invoke();

            currentPage = page;
            EnablePageObjects(ref currentPage, true);
            currentPage.OnOpenEvent.Invoke();
            return true;
        }

        return false;
    }

    public void ClearPageHistory() => pageHistory.Clear();
    public int GetPageCount() => uiPages.Count;
    public int GetHistoryCount() => pageHistory.Count;
    public bool PageExist(string pageName) => !(GetPageIndex(pageName) == -1);

    public MenuPage GetPage(string pageName)
    {
        int index = GetPageIndex(pageName);
        return (index == -1) ? EmptyPage : uiPages[index];
    }

    public MenuPage GetPage(int index)
    {
        if (index < 0 || index >= uiPages.Count)
        {
            Debug.LogError($"No page exist at index ({index})");
            return EmptyPage;
        }

        return uiPages[index];
    }

    public MenuPage[] GetPages() => uiPages.ToArray();

    [System.Serializable]
    public struct MenuPage
    {

        public MenuPage(string pageName, bool overlay, GameObject[] pageObjects, UnityEvent onOpenEvent, UnityEvent onCloseEvent)
        {
            PageName = pageName;
            Overlay = overlay;
            PageObjects = pageObjects;
            OnOpenEvent = onOpenEvent;
            OnCloseEvent = onCloseEvent;
        }

        public string PageName { get; private set; }
        public bool Overlay { get; private set; }
        public GameObject[] PageObjects { get; private set; }
        public UnityEvent OnOpenEvent { get; private set; }
        public UnityEvent OnCloseEvent { get; private set; }

    }

}
