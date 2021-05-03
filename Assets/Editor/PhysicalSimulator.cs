using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


/*
 3- Physical Simulator

Criar um script que permita aplicar f�sica a game objects no edit mode.

Funcionamento:

- Criar a op��o �Automation� > �Physical Simulator� no menu superior da Unity;

- Ao clicar em �Physical Simulator�, abrir uma nova janela contendo dois bot�es, 
uma campo de lista de game objects, um texto descrevendo o estado da simula��o (Ativada / Desativada);

- Bot�o "Run" para iniciar a simula��o de f�sica;

- Bot�o "Stop" para encerrar a simula��o de f�sica;

- Quando a simula��o for iniciada game objects inclu�dos da lista devem receber mesh collider e rigidbody caso n�o tenham;

- Quando a simula��o for encerrada os componentes adicionados na inicializa��o devem ser removidos.
 
*/


namespace Automation
{

    [ExecuteInEditMode]
    public class PhysicalSimulator : EditorWindow
    {
        bool isRunning;

        [SerializeField]
        List<GameObject> _selectedGameObjects = new List<GameObject>();

        //Wrapper
        private SerializedObject so;
        private SerializedProperty stringsProp;


        [MenuItem("Automation/Physical Simulator", false, 0)]
        public static void OpenSimulationWindow()
        {

            PhysicalSimulator window = (PhysicalSimulator)EditorWindow.GetWindow(typeof(PhysicalSimulator));
            window.Show();
        }

        private float timer;
        private void Update()
        {
            if( !Application.isPlaying && isRunning )
            {
                SimulatePhysics();
            }
        }

        private void SimulatePhysics()
        {
            if (Physics.autoSimulation)
                return; // do nothing if the automatic simulation is enabled

            timer += Time.deltaTime;

            while (timer >= Time.fixedDeltaTime)
            {
                timer -= Time.fixedDeltaTime;
                Physics.Simulate(Time.fixedDeltaTime);
            }
        }

        void OnGUI()
        {
            GUILayout.Label("Simula��o Status", EditorStyles.boldLabel);

            GUILayout.Label(isRunning ? "Ativada" : "Desativada");

            EditorGUILayout.Space();

            BuildListObjectProp();

            EditorGUILayout.Space();

            BuildButtons();

        }

        private void BuildButtons()
        {
            GUILayout.BeginHorizontal();

            if (!isRunning)
            {
                if (GUILayout.Button("Run"))
                {
                    isRunning = true;
                    RunPhysicalSimulator();
                }
            }
            else
            {
                if (GUILayout.Button("Stop"))
                {
                    isRunning = false;
                    StopPhysicalSimulator();
                }
            }

            GUILayout.EndHorizontal();
        }

        private void BuildListObjectProp()
        {
            EditorGUILayout.BeginToggleGroup("GameObject List", !isRunning);

                EditorGUI.indentLevel++;
                    //List of objects
                    EditorGUILayout.PropertyField(stringsProp, true);
                    so.ApplyModifiedProperties();
                EditorGUI.indentLevel--;

            EditorGUILayout.EndToggleGroup();
        }

        private void RunPhysicalSimulator()
        {

            Physics.autoSimulation = false;

            //Add physics components
            for (int i = 0; i < _selectedGameObjects.Count; i++)
            {
                AddComponentOfType(_selectedGameObjects[i], typeof(Rigidbody));
                AddComponentOfType(_selectedGameObjects[i], typeof(MeshCollider));
            }

        }

        private void StopPhysicalSimulator()
        {

            Physics.autoSimulation = true;

            for (int i = 0; i < _selectedGameObjects.Count ; i++)
            {
                RemoveComponentOfType(_selectedGameObjects[i], typeof(Rigidbody));
                RemoveComponentOfType(_selectedGameObjects[i], typeof(Rigidbody2D));
            }

            //TODO restore inital position ?
        }

        private void AddComponentOfType(GameObject obj, Type type)
        {
            //add once
            if (obj.GetComponent(type) != null) return;

            obj.AddComponent(type);

            //Default for MeshCollider component
            if ( type == typeof (MeshCollider) )
            {
                MeshCollider meshCollider = (MeshCollider) obj.GetComponent(type);
                if (meshCollider != null)
                    meshCollider.convex = true;
            }
        }

        private void RemoveComponentOfType( GameObject obj , Type type)
        {
            var component = obj.GetComponent(type);

            if(component != null)
            {
                DestroyImmediate(component);
            }
        }

        private void OnEnable()
        {
            so = new SerializedObject(this);
            stringsProp = so.FindProperty("_selectedGameObjects");
        }

    }
}
