using UnityEditor;
using UnityEngine;


/*
 1- Script Cleaner

Criar um script que permita excluir todos os scripts de um game object e seus filhos.

Funcionamento:

- Ao clicar com o botão direito do mouse em um game object na hierarquia aparece a opção no menu “Automation” > “Remove scripts”;

- Ao selecionar "Remove scripts" todos os scripts associados ao game object e seus filhos são deletados;

- Após a exclusão aparece uma mensagem de confirmação.
 */

namespace Automation
{

    public class ScriptCleaner : MonoBehaviour
    {

        [MenuItem("GameObject/Automation/Remove scripts", false, 0)]
        public static void RemoveScripts()
        {
            if (Selection.activeObject == null) return;

            GameObject currentGameObj = Selection.activeObject as GameObject;

            //Get components        
            MonoBehaviour[] components= currentGameObj.GetComponentsInChildren<MonoBehaviour>();
            for (int i = 0; i < components.Length; i++)
            {
                DestroyImmediate(components[i].GetComponent<MonoBehaviour>());
            }

            //Alert
            EditorUtility.DisplayDialog( "Warning", "Scripts has been removed from " + currentGameObj.name , "Close" );
        

        }

        [MenuItem("GameObject/Automation/Remove scripts and childreen", false, 0)]
        public static void RemoveScriptsAndChildreen()
        {
            if (Selection.activeObject == null) return;

            GameObject currentGameObj = Selection.activeObject as GameObject;

            //Destroy all Childreen using a reverse for loop               
            for (int i = currentGameObj.transform.childCount - 1; i >= 0; --i)
            {
                DestroyImmediate(currentGameObj.transform.GetChild(i).gameObject);
            }

            //Get components        
            MonoBehaviour[] components = currentGameObj.GetComponents<MonoBehaviour>();
            for (int i = 0; i < components.Length; i++)
            {
                DestroyImmediate(components[i].GetComponent<MonoBehaviour>());
            }

            //Alert
            EditorUtility.DisplayDialog("Warning", "Scripts and childrenn has been removed from " + currentGameObj.name, "Close");



        }
    }

}
