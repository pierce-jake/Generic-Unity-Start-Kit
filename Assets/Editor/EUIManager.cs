using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

[CustomEditor(typeof(UIManager))]
public class EUIManager : Editor
{

    private string swapPageName = "page_0";
    private int swapIndex = 0;
    private string pageName = "page_0";
    private bool overlay;
    private GameObject[] pageObjects;
    private UnityEvent onOpenEvent, onCloseEvent;

    public void Init()
    {
        EditorGUILayout.BeginHorizontal();
        //pageName = EditorGUILayout.TextField($"page_{ui.GetPageCount()}");
        EditorGUILayout.EndHorizontal();
    }

    public override void OnInspectorGUI()
    {
        UIManager ui = (UIManager)target;

        ui.SortedPages = EditorGUILayout.Toggle("Enable Sorting", ui.SortedPages);
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(ui.SortedPages);
        swapPageName = EditorGUILayout.TextField(swapPageName);
        swapIndex = EditorGUILayout.IntField(swapIndex);
        if(GUILayout.Button("Swap Index"))
        {

        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Page Editor");

        //Page Buttons
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Create Page"))
            CreatePage(ref ui);

        else if (GUILayout.Button("Remove Page"))
            RemovePage(ref ui);

        else if (GUILayout.Button("Modify Page"))
            ModifyPage(ref ui);

        else if (GUILayout.Button("Get Page"))
            GetPage(ref ui);

        EditorGUILayout.EndHorizontal();

        //Modify Info
        pageName = EditorGUILayout.TextField(pageName);
        overlay = EditorGUILayout.Toggle("Overlay Page", overlay);

        EditorGUILayout.LabelField("--==UI Page List==--");
        DisplayPageList(ref ui);
        //pageObjects = EditorGUILayout.ObjectField(pageObjects);

        //EditorGUILayout.BeginHorizontal();
        //pageName = EditorGUILayout.TextField($"page_{ui.GetPageCount()}");
        //EditorGUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }

    private void CreatePage(ref UIManager ui)
    {
        if (ui.PageExist(pageName))
        {
            Debug.LogError("Couldn't create UI page as it already exist! ");
            return;
        }

        if (ui.AddMenuPage(ref pageName, overlay, ref pageObjects, ref onOpenEvent, ref onCloseEvent))
        {
            Debug.Log($"Page \"{pageName}\" created successfully.");
        }
        else
        {
            Debug.LogError("Couldn't create page as name space is empty");
        }
    }

    private void RemovePage(ref UIManager ui)
    {
        if (!ui.RemoveMenuPage(pageName))
        {
            Debug.LogError($"Couldn't find page of name \"{pageName}\"");
            return;
        }

        Debug.Log("Page removed successfully");
    }

    private void ModifyPage(ref UIManager ui)
    {
        if (!ui.RemoveMenuPage(pageName))
        {
            Debug.LogError($"Unable to modify page \"{pageName}\" as it does not exist.");
            return;
        }

        ui.AddMenuPage(ref pageName, overlay, ref pageObjects, ref onOpenEvent, ref onCloseEvent);
    }

    private void GetPage(ref UIManager ui)
    {
        if (!ui.PageExist(pageName))
        {
            Debug.LogError($"Couldn't find page of name \"{pageName}\"");
            return;
        }

        UIManager.MenuPage page = ui.GetPage(pageName);

        pageName = page.PageName;
        overlay = page.Overlay;
        pageObjects = page.PageObjects;
        onOpenEvent = page.OnOpenEvent;
        onCloseEvent = page.OnCloseEvent;
    }

    private void DisplayPageList(ref UIManager ui)
    {
        UIManager.MenuPage[] pages = ui.GetPages();

        for (int i = 0; i < pages.Length; i++)
        {
            EditorGUILayout.LabelField($"{i}: {pages[i].PageName}");
        }

    }

}
