using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Linq;


namespace com.ootii.Messages {

    public class RoutineObject {
        public int index;
        public Coroutine routine;
        public MonoBehaviour parent;
        public string parentName;

        public RoutineObject(int _id, Coroutine _routine, MonoBehaviour _parent) {
            index = _id;
            routine = _routine;
            parent = _parent;
            if (parent != null) {
                parentName = parent.name;
            }
        }
    }
    /// <summary>
    /// Static class that allows messages to be dispatched
    /// from one object to another. These messages can be
    /// sent immediately, the next frame, or set for a delay.
    /// </summary>
    public class MessageDispatcher {


        public static Dictionary<object, Dictionary<string, List<MessageListenerDefinition>>> ListenerBook = new Dictionary<object, Dictionary<string, List<MessageListenerDefinition>>>();
        public static Dictionary<string, Dictionary<object, List<MessageListenerDefinition>>> MessageHandlerBook = new Dictionary<string, Dictionary<object, List<MessageListenerDefinition>>>();
        public static Dictionary<object, Dictionary<Button, UnityAction>> ClickBook = new Dictionary<object, Dictionary<Button, UnityAction>>();
        public static Dictionary<object, Dictionary<object, UnityAction<string>>> OnChangedBook = new Dictionary<object, Dictionary<object, UnityAction<string>>>();
        public static Dictionary<object, Dictionary<object, UnityAction<float>>> Slider_OnChangedBook = new Dictionary<object, Dictionary<object, UnityAction<float>>>();
        public static Dictionary<object, UnityAction> SecondTimer = new Dictionary<object, UnityAction>();

        public static Dictionary<MonoBehaviour,List<int>> CoroutineBook = new Dictionary<MonoBehaviour, List<int>>();
        private static Dictionary<int, RoutineObject> CoroutineIndex = new Dictionary<int, RoutineObject>();

        private static int RoutineCounter = 0;

        private static int ProblematicRoutineRemovals = 0;

        #region routines
        public static int BeginCoroutine(MonoBehaviour parent, IEnumerator IEnum) {
            int newIndex = RoutineCounter + 1; //Need new int reference, as RoutineCounter can be called from different threads and the value can change between operations due to nested coroutines
            RoutineCounter++;
            if (!CoroutineBook.ContainsKey(parent)) {
                CoroutineBook.Add(parent, new List<int>());
            }
   
            //if (CoroutineIndex.ContainsKey(newIndex)) {
            //    Debug.LogError("??");
            //}
            
            IEnumerator ie = StartCo(newIndex, IEnum);

            CoroutineBook[parent].Add(newIndex);
            Coroutine c = parent.StartCoroutine(ie);
            
            CoroutineIndex.Add(newIndex, new RoutineObject(newIndex, c, parent));
            return newIndex;
        }

        private static RoutineObject GetRoutineObject(int id) {
            return CoroutineIndex[id];
        }

        private static IEnumerator StartCo(int id, IEnumerator ie) {
            yield return ie;
            RemoveCoroutine(GetRoutineObject(id));
        }


        private static void RemoveCoroutine(RoutineObject ro) {
            CoroutineIndex.Remove(ro.index);
            if (CoroutineBook.ContainsKey(ro.parent)) {
                CoroutineBook[ro.parent].Remove(ro.index);
                if (CoroutineBook[ro.parent].Count == 0) {
                    CoroutineBook.Remove(ro.parent);
                }
            }
        }



        public static List<string> ProblematicRoutineNames = new List<string>();

        public static void EndCoroutine(int id, bool remove=true) {
            if (CoroutineIndex.ContainsKey(id)) {
                RoutineObject ro = CoroutineIndex[id];
                if (ro.parent != null) {
                    ro.parent.StopCoroutine(ro.routine);
                } else {
                    ProblematicRoutineNames.Add(ro.parentName);
                    ProblematicRoutineRemovals++;
                }
                if (remove) {
                    RemoveCoroutine(ro);
                    //ro = null;
                }
            } else {
#if UNITY_EDITOR
                //Debug.LogError("Removing coroutine that doesn't exist");
#endif
            }
            
        }

        public static Action<string> ClickMasterAction = null;
        public static void SetMasterClickEvent(Action<string> cb) {
            ClickMasterAction = cb;
        }
        

        private static void ClearAllCoroutinesFromParent(MonoBehaviour parent) {
            if (CoroutineBook.ContainsKey(parent)) {
                foreach (int id in CoroutineBook[parent]) {
                    EndCoroutine(id,false);
                    CoroutineIndex.Remove(id);
                }
                CoroutineBook.Remove(parent);
            }
            parent.StopAllCoroutines();

            foreach(MonoBehaviour mb in CoroutineBook.Keys.ToList()) {
                if (mb.ToString() == "null") {
                    foreach (int ii in  CoroutineBook[mb].ToList()) {
                        EndCoroutine(ii);
                    }
                    CoroutineBook.Remove(mb);
                }
            }
            
        }
        #endregion

        /// <summary>
        /// Determines if the message recipient is determined by
        /// the listening object's name or listening object's tags.
        /// </summary>
        private static int mRecipientType = EnumMessageRecipientType.NAME;
        public static int RecipientType {
            get { return mRecipientType; }
            set { mRecipientType = value; }
        }

        /// <summary>
        /// Determines if the dispatcher reports unhandled messages
        /// </summary>
        public static bool ReportUnhandledMessages = false;

        /// <summary>
        /// Allows the caller to assign a hander for when messages have no recipient
        /// </summary>
        public static MessageHandler MessageNotHandled = null;

        /// <summary>
        /// Used to ensure we send messages the 'next update'
        /// </summary>
        public static int FrameIndex = 0;

        /// <summary>
        /// Create the MessengerStub at startup and tie it into the Unity update path
        /// </summary>
#pragma warning disable 0414
        private static MessageDispatcherStub sStub = (new GameObject("MessageDispatcherStub")).AddComponent<MessageDispatcherStub>();
#pragma warning restore 0414

        /// <summary>
        /// List of messages that are being held until it's time for them to be dispatched
        /// </summary>
        private static List<IMessage> mMessages = new List<IMessage>();


        /// <summary>
        /// Clears all messages from the queue
        /// </summary>
        public static void ClearMessages() {
            mMessages.Clear();
            
        }

        /// <summary>
        /// Clears all listeners from the messenger
        /// </summary>
        public static void ClearListeners() {
            List<object> keys = new List<object>(ListenerBook.Keys);
            foreach (object o in keys) {
                RemoveAllListenersFromParent(o);
            }

            ListenerBook.Clear();

        }
#region Click Events
        public static void AddClickEvent(object parent, Button button, Action cb,string MasterStringVar="") {
            if (!ClickBook.ContainsKey(parent)) {
                ClickBook.Add(parent, new Dictionary<Button, UnityAction>());
            }
            UnityAction action = new UnityAction(() => {

                cb(); //Callback

                //PlayAudio
                if (ClickMasterAction != null) {
                    ClickMasterAction(MasterStringVar);
                }
            });
            if (!ClickBook[parent].ContainsKey(button)) {
                ClickBook[parent].Add(button, action);
            } else {
#if UNITY_EDITOR
                Debug.LogError("Multiple button events: " + parent.ToString() + " : " + button.name);
#endif
                button.onClick.RemoveAllListeners();
                ClickBook[parent][button] = null;
                ClickBook[parent].Remove(button);
                ClickBook[parent].Add(button, action);
            }
            button.onClick.AddListener(action);
        }

        
        public static void AddSecondTimer(object parent, Action cb) {
            if (parent == null) {
                Debug.LogError("AddSecondTimerNull");
                return;
            }
            if(cb == null) {
                Debug.LogError("AddSecondTimerActionNull");
                return;
            }
            if (SecondTimer.ContainsKey(parent)) {
                Debug.LogError("Multiple Second Timers on " + parent.ToString());
                SecondTimer.Remove(parent);
            }
            SecondTimer.Add(parent, new UnityAction(cb));
            try {
                cb(); //Process it right away...why not
            } catch (Exception ee) {
                Debug.LogError("AddSecondTimer: " + parent.ToString());
            }
        }

        public static void CallSecondTimer() {
            foreach(UnityAction cb in SecondTimer.Values) {
                try {
                    cb();
                } catch (Exception ee) {
                    Debug.LogWarning("CallSecondTimer Error");
                }
            }
        }

        public static void RemoveAllClickEvents() {
            List<object> keys = new List<object>(ClickBook.Keys);
            foreach (object o in keys) {
                RemoveClickEvents(o);
            }
        }
        public static void RemoveClickEvents(object parent) {

            if (ClickBook.ContainsKey(parent)) {
                List<Button> keys = new List<Button>(ClickBook[parent].Keys);
                foreach (Button b in keys) {
                    ClickBook[parent].Remove(b);
                    b.onClick.RemoveAllListeners();
                }
                ClickBook.Remove(parent);

            }
            foreach(object o in ClickBook.Keys.ToList()) {
                if (o.ToString() == "null") {
                    ClickBook.Remove(o);
                }
            }
            //ClickBook.Remove((object)null);

        }
        private static void ResetDictionary() {

        }
        public static void RemoveClickEvents(object parent, Button button) {
            if (ClickBook.ContainsKey(parent)) {
                if (ClickBook[parent].ContainsKey(button)) {
                    ClickBook[parent].Remove(button);
                    button.onClick.RemoveAllListeners();
                }
            }
        }
        #endregion


        #region OnChanged Events
        public static void AddOnChangedEvent(object parent, TMP_InputField obj, Action<string> cb) {
            if (!OnChangedBook.ContainsKey(parent)) {
                OnChangedBook.Add(parent, new Dictionary<object, UnityAction<string>>());
            }
            UnityAction<string> action = new UnityAction<string>(cb);
            if (!OnChangedBook[parent].ContainsKey(obj)) {
                OnChangedBook[parent].Add(obj, action);
            } else {
                obj.onValueChanged.RemoveAllListeners();
#if UNITY_EDITOR
                Debug.LogError("Multiple on change events: " + parent.ToString() + " : " + obj.name);
#endif
                OnChangedBook[parent][obj] = null;
                OnChangedBook[parent].Remove(obj);
                OnChangedBook[parent].Add(obj, action);
            }
            obj.onValueChanged.AddListener(action);
        }
        public static void AddOnChangedEvent(object parent, Slider obj, Action<float> cb) {
            if (!Slider_OnChangedBook.ContainsKey(parent)) {
                Slider_OnChangedBook.Add(parent, new Dictionary<object, UnityAction<float>>());
            }
            UnityAction<float> action = new UnityAction<float>(cb);
            if (!Slider_OnChangedBook[parent].ContainsKey(obj)) {
                Slider_OnChangedBook[parent].Add(obj, action);
            } else {
                obj.onValueChanged.RemoveAllListeners();
#if UNITY_EDITOR
                Debug.LogError("Multiple on change events (slider): " + parent.ToString() + " : " + obj.name);
#endif
                Slider_OnChangedBook[parent][obj] = null;
                Slider_OnChangedBook[parent].Remove(obj);
                Slider_OnChangedBook[parent].Add(obj, action);
            }
            obj.onValueChanged.AddListener(action);
        }





        public static void RemoveAllOnChangeEvents_InputField() {
            List<object> keys = new List<object>(OnChangedBook.Keys);
            foreach (object o in keys) {
                RemoveOnChangeEvents(o);
            }
        }
        public static void RemoveAllOnChangeEvents_Sliders() {
            List<object> keys = new List<object>(Slider_OnChangedBook.Keys);
            foreach (object o in keys) {
                RemoveOnChangeEvents(o);
            }
        }
        public static void RemoveOnChangeEvents(object parent) {
            if (OnChangedBook.ContainsKey(parent)) {
                List<object> keys = new List<object>(OnChangedBook[parent].Keys);
                foreach (TMP_InputField b in keys) {
                    OnChangedBook[parent].Remove(b);
                    b.onValueChanged.RemoveAllListeners();
                }
            }

            if (Slider_OnChangedBook.ContainsKey(parent)) {
                List<object> keys = new List<object>(Slider_OnChangedBook[parent].Keys);
                foreach (Slider b in keys) {
                    Slider_OnChangedBook[parent].Remove(b);
                    b.onValueChanged.RemoveAllListeners();
                }
            }

            foreach(object o in OnChangedBook.Keys.ToList()) {
                if(o.ToString() == "null") {
                    OnChangedBook.Remove(o);
                }
            }

        }
        public static void RemoveOnChangeEvents(object parent, TMP_InputField intputfield) {
            if (OnChangedBook.ContainsKey(parent)) {
                if (OnChangedBook[parent].ContainsKey(intputfield)) {
                    OnChangedBook[parent].Remove(intputfield);
                    intputfield.onValueChanged.RemoveAllListeners();
                }
            }
        }
        public static void RemoveOnChangeEvents(object parent, Slider intputfield) {
            if (Slider_OnChangedBook.ContainsKey(parent)) {
                if (Slider_OnChangedBook[parent].ContainsKey(intputfield)) {
                    Slider_OnChangedBook[parent].Remove(intputfield);
                    intputfield.onValueChanged.RemoveAllListeners();
                }
            }
        }
        #endregion

        /// <summary>
        /// Tie the handler to the specified message type. 
        /// This way it will be raised when a message of this type comes in. This
        /// method allows us to listen for anyone's messages
        /// </summary>
        /// <param name="rMessageType">Message we want to listen for</param>
        /// <param name="rHandler">Hander to handle the message</param>
        public static void AddListener(object parent, string rMessageType, MessageHandler rHandler) {
            if (parent == null) {
#if UNITY_EDITOR
                Debug.LogError("attempt to add listener of null parent: " + rMessageType);
#endif
                return; 
            }
            MessageListenerDefinition lListener = MessageListenerDefinition.Allocate();
            lListener.Parent = parent;
            lListener.MessageType = rMessageType;
            lListener.Handler = rHandler;
            lListener.AddedAt = DateTime.Now;

            AddListenerToBook(parent, lListener);
        }


        private static void AddListenerToBook(object parent, MessageListenerDefinition mld) {
            #region Add to ListenerBook
            if (!ListenerBook.ContainsKey(parent)) {
                ListenerBook.Add(parent, new Dictionary<string, List<MessageListenerDefinition>>());

            }
            if (!ListenerBook[parent].ContainsKey(mld.MessageType)) {
                ListenerBook[parent].Add(mld.MessageType, new List<MessageListenerDefinition>());
            }
            ListenerBook[parent][mld.MessageType].Add(mld);
            #endregion

            #region Add to HandlerBook
            if (!MessageHandlerBook.ContainsKey(mld.MessageType)) {
                MessageHandlerBook.Add(mld.MessageType, new Dictionary<object, List<MessageListenerDefinition>>());
            }
            if (!MessageHandlerBook[mld.MessageType].ContainsKey(parent)) {
                MessageHandlerBook[mld.MessageType].Add(parent, new List<MessageListenerDefinition>());
            }
            MessageHandlerBook[mld.MessageType][parent].Add(mld);
            #endregion
        }

        private static void RemoveListenerFromBook(object parent, string messageType) {

            if (!ListenerBook.ContainsKey(parent)) {
                Debug.LogError("RemoveListenerFromBook :: Missing Parent :: " + parent.ToString());
                return;
            }
            if (!ListenerBook[parent].ContainsKey(messageType)) {
                Debug.LogError("RemoveListenerFromBook :: Missing Key :: " + parent.ToString() + "." + messageType);
                return;
            }
            //Clear listener from memory
            foreach (MessageListenerDefinition mld in ListenerBook[parent][messageType].ToArray()) {
                MessageListenerDefinition.Release(mld);
            }


            ListenerBook[parent][messageType].Clear(); //Clear definitions from list
            ListenerBook[parent].Remove(messageType); //Remove list entirely from parent

            //Remove from handler book as well
            RemoveListenerFromHandlerBook(parent, messageType);

        }
        private static void RemoveListenerFromHandlerBook(object parent, string messageType) {
            if (!MessageHandlerBook.ContainsKey(messageType)) {
                Debug.LogError("RemoveListenerFromHandlerBook :: Missing MessageType :: " + parent.ToString() + "." + messageType);
                return;
            }
            if (!MessageHandlerBook[messageType].ContainsKey(parent)) {
                Debug.LogError("RemoveListenerFromHandlerBook :: Missing Parent :: " + parent.ToString() + "." + messageType);
                return;
            }
            MessageHandlerBook[messageType].Remove(parent);

        }
        public static void RemoveAllListenersFromParent(object parent, bool lastTime = false) {
            RemoveClickEvents(parent);
            RemoveOnChangeEvents(parent);
            if ((MonoBehaviour)parent != null) {
                ClearAllCoroutinesFromParent((MonoBehaviour)parent);
            }

            if(SecondTimer.ContainsKey(parent)) {
                SecondTimer.Remove(parent);
            }

            if (!ListenerBook.ContainsKey(parent)) {
                //Debug.LogError("RemoveListenerFromBook :: Missing Parent :: " + parent.ToString());
                return;
            }
            

            Dictionary<string, List<MessageListenerDefinition>> dicList = ListenerBook[parent];
            foreach (List<MessageListenerDefinition> list in ListenerBook[parent].Values) {
                foreach (MessageListenerDefinition mld in list) {
                    if (MessageHandlerBook.ContainsKey(mld.MessageType)) {
                        MessageHandlerBook[mld.MessageType].Remove(parent);
                        if (MessageHandlerBook[mld.MessageType].Count == 0) {
                            MessageHandlerBook.Remove(mld.MessageType);
                        }
                    }

                    MessageListenerDefinition.Release(mld);

                    
                }
            }

            ListenerBook.Remove(parent);

            foreach (object o in ListenerBook.Keys.ToList()) {
                if (o.ToString() == "null") {
                    ListenerBook.Remove(o);
                }
            }

            

            /*
            if (!lastTime) {
                if (ListenerBook.ContainsKey(null)) {
                    RemoveAllListenersFromParent(null, true);
                }
            }*/
        }


        /// <summary>
        /// Stop listening for messages for the specified type.
        /// </summary>
        /// <param name="rMessageType">Message we want to listen for</param>
        /// <param name="rHandler">Hander to handle the message</param>
        /// <param name="rImmediate">Determines if the function ignores the cache and forcibly removes the listener from the list now</param>
        public static void RemoveListener(object parent, string rMessageType, MessageHandler rHandler) {
            RemoveListenerFromBook(parent, rMessageType);

            //RemoveListener(rMessageType, "", rHandler, false);
        }

        #region SendMessages
        /// <summary>
        /// Create and send a message object
        /// </summary>
        /// <param name="rType">Type of message to send</param>
        public static void SendMessage(string rType, float rDelay = 0f) {
            // Create the message
            Message lMessage = Message.Allocate();

            lMessage.Sender = null;
            lMessage.Recipient = "";
            lMessage.Type = rType;
            lMessage.Data = null;
            lMessage.Delay = rDelay;

            // Send it or store it
            SendMessage(lMessage);

            // Free up the message since we created it
            if (rDelay == EnumMessageDelay.IMMEDIATE) { lMessage.Release(); }
        }

        /// <summary>
        /// Create and send a message object
        /// </summary>
        /// <param name="rType">Type of message to send</param>
        /// <param name="rFilter">Filter to send only to those listening to the filter</param>
        public static void SendMessage(string rType, string rFilter, float rDelay = 0f) {
            // Create the message
            Message lMessage = Message.Allocate();
            lMessage.Sender = null;
            lMessage.Recipient = rFilter;
            lMessage.Type = rType;
            lMessage.Data = null;
            lMessage.Delay = rDelay;

            // Send it or store it
            SendMessage(lMessage);

            // Free up the message since we created it
            if (rDelay == EnumMessageDelay.IMMEDIATE) { lMessage.Release(); }
        }

        ///<summary>
        /// Create and send a message object
        ///</summary>
        ///<param name="rType">Type of message to send</param>
        ///<param name="rData">Data to send</param>
        public static void SendMessageData(string rType, object rData, float rDelay = 0f) {
            // Create the message
            Message lMessage = Message.Allocate();
            lMessage.Sender = null;
            lMessage.Recipient = "";
            lMessage.Type = rType;
            lMessage.Data = rData;
            lMessage.Delay = rDelay;

            // Send it or store it
            SendMessage(lMessage);

            // Free up the message since we created it
            if (rDelay == EnumMessageDelay.IMMEDIATE) { lMessage.Release(); }
        }

        /// <summary>
        /// Create and send a message object
        /// </summary>
        /// <param name="rSender">Sender</param>
        /// <param name="rType">Type of message to send</param>
        /// <param name="rData">Data to send</param>
        /// <param name="rDelay">Seconds to delay</param>
        public static void SendMessage(object rSender, string rType, object rData, float rDelay) {
            // Create the message
            Message lMessage = Message.Allocate();
            lMessage.Sender = rSender;
            lMessage.Recipient = "";
            lMessage.Type = rType;
            lMessage.Data = rData;
            lMessage.Delay = rDelay;

            // Send it or store it
            SendMessage(lMessage);

            // Free up the message since we created it
            if (rDelay == EnumMessageDelay.IMMEDIATE) { lMessage.Release(); }
        }

        /// <summary>
        /// Create and send a message object
        /// </summary>
        /// <param name="rSender">Sender</param>
        /// <param name="rRecipient">Recipient to send to</param>
        /// <param name="rType">Type of message to send</param>
        /// <param name="rData">Data to send</param>
        /// <param name="rDelay">Seconds to delay</param>
        public static void SendMessage(object rSender, object rRecipient, string rType, object rData, float rDelay) {
            // Create the message
            Message lMessage = Message.Allocate();
            lMessage.Sender = rSender;
            lMessage.Recipient = (rRecipient != null ? rRecipient : "");
            lMessage.Type = rType;
            lMessage.Data = rData;
            lMessage.Delay = rDelay;

            // Send it or store it
            SendMessage(lMessage);

            // Free up the message since we created it
            if (rDelay == EnumMessageDelay.IMMEDIATE) { lMessage.Release(); }
        }

        /// <summary>
        /// Create and send a message object
        /// </summary>
        /// <param name="rSender">Sender</param>
        /// <param name="rRecipient">Recipient name to send to</param>
        /// <param name="rType">Type of message to send</param>
        /// <param name="rData">Data to send</param>
        /// <param name="rDelay">Seconds to delay</param>
        public static void SendMessage(object rSender, string rRecipient, string rType, object rData, float rDelay) {
            // Create the message
            Message lMessage = Message.Allocate();
            lMessage.Sender = rSender;
            lMessage.Recipient = rRecipient;
            lMessage.Type = rType;
            lMessage.Data = rData;
            lMessage.Delay = rDelay;

            // Send it or store it
            SendMessage(lMessage);

            // Free up the message since we created it
            if (rDelay == EnumMessageDelay.IMMEDIATE) { lMessage.Release(); }
        }


        /// <summary>
        /// Send the message object as needed. In this instance, the caller needs to
        /// release the message.
        /// </summary>
        /// <param name="rMessage">Message to send</param>
        /// <param name="rSetUnhandledToHandled">Determines if we set an unhandled message to 'handled'.</param>
        public static void SendMessage(IMessage rMessage, bool rSetUnhandledToHandled = false) {
            if (rMessage == null) { return; }

            bool lReportMissingRecipient = true;

            // Use the ID of the message as the default type
            if (rMessage.Type.Length == 0 && rMessage.ID != 0) {
                rMessage.Type = rMessage.ID.ToString();
            }

            // Hold the message for the delay or the next frame (< 0)
            if (rMessage.Delay > 0 || rMessage.Delay < 0) {
                if (!mMessages.Contains(rMessage)) {
                    rMessage.FrameIndex = FrameIndex;
                    mMessages.Add(rMessage);
                }

                lReportMissingRecipient = false;
            } else if (MessageHandlerBook.ContainsKey(rMessage.Type)) { // Send the message now if there are handlers
                // Get handlers for the message type
                Dictionary<object, List<MessageListenerDefinition>> lHandlers = MessageHandlerBook[rMessage.Type];

                foreach (List<MessageListenerDefinition> list in lHandlers.Values.ToList()) {
                    foreach (MessageListenerDefinition mld in list.ToList()) {
                        if (mld.Handler != null) {
                            rMessage.IsSent = true;
                            mld.Handler(rMessage);
                            lReportMissingRecipient = false;
                        }
                    }
                }

            }

            // If we were unable to send the message, we may need to report it
            if (lReportMissingRecipient) {
                if (ReportUnhandledMessages) {
                    if (MessageNotHandled == null) {
#if UNITY_EDITOR
                        Debug.LogWarning("MessageDispatcher: Unhandled Message of type " + rMessage.Type);
#endif
                    } else {
                        MessageNotHandled(rMessage);
                    }
                }

                // Flag the message as handled so we can remove it
                if (!rMessage.IsHandled) { rMessage.IsHandled = rSetUnhandledToHandled; }
            }
        }
        #endregion

        public static string GetDebugInfo() {
            string str = "";
            try {
                foreach (object o in ListenerBook.Keys) {
                    Dictionary<string, List<MessageListenerDefinition>> dic = ListenerBook[o];
                    int count = 0;
                    foreach (List<MessageListenerDefinition> list in dic.Values) {
                        count += list.Count;
                    }
                    str += ":::" + o.ToString() + "=" + count + " Events\n";
                }

                str += "\n\n------Events------\n";
                foreach (string s in MessageHandlerBook.Keys) {
                    Dictionary<object, List<MessageListenerDefinition>> dic = MessageHandlerBook[s];
                    int count = 0;
                    foreach (List<MessageListenerDefinition> list in dic.Values) {
                        count += list.Count;
                    }
                    str += ":::" + s + "=" + count + " parents\n";
                }


                str += "\n\n-----CLICK EVENTS----\n";
                foreach (object o in ClickBook.Keys) {
                    if (o == null) {
                        str += "::Null Ref\n";
                    } else {
                        Dictionary<Button, UnityAction> dic = ClickBook[o];
                        foreach (Button button in dic.Keys) {
                            if (button == null) {
                                str += "::" + o.ToString() + " :: NullButton\n";
                            } else {
                                str += "::" + o.ToString() + " :: " + button.name + "\n";
                            }
                        }
                    }
                }

                str += "\n\n-----Routines----\n";
                foreach (MonoBehaviour o in CoroutineBook.Keys) {
                    
                    str += "::" + o.ToString() + " :: " + CoroutineBook[o].Count + " Routines \n";
   
                }

                str += "\n\n-----Broken Routines [Bad Parent Removal] (x" + ProblematicRoutineRemovals + ")-----\n";
                foreach(string ss in ProblematicRoutineNames) {
                    str += "::" + ss + "\n";
                }
            } catch (Exception ee) {
                Debug.LogError("GetDebugInfo :: " + ee.Message + " :: " + ee.StackTrace);

            }
            return str;
        }


        public static string GetDebugParents() {
            string str = "";
            str += "\n\n------Events------\n";
            foreach (string s in MessageHandlerBook.Keys) {
                Dictionary<object, List<MessageListenerDefinition>> dic = MessageHandlerBook[s];
                int count = 0;
                str += ":::" + s + ":::\n";
                foreach (object o in dic.Keys) {
                    str += "-" + o.ToString() + " [count=" + dic[o].Count + "]\n"; 
                }
                
            }
            return str;
        }
        

        /// <summary>
        /// Raised each tick so we can determine if it's time to send delayed messages
        /// </summary>
        public static void Update() {
            // Process the messages and determine if it's time to send them
            for (int i = 0;i < mMessages.Count;i++) {
                IMessage lMessage = mMessages[i];

                // Check if we're sending based on the next update
                if (lMessage.Delay == EnumMessageDelay.NEXT_UPDATE) {
                    if (lMessage.FrameIndex < FrameIndex) {
                        lMessage.Delay = EnumMessageDelay.IMMEDIATE;
                    }
                }
                // Otherwise, we may be time based
                else {
                    // Reduce the delay
                    lMessage.Delay -= Time.deltaTime;
                    if (lMessage.Delay < 0) {
                        lMessage.Delay = EnumMessageDelay.IMMEDIATE;
                    }
                }

                // If it's time, send the message and flag for removal
                if (!lMessage.IsSent && lMessage.Delay == EnumMessageDelay.IMMEDIATE) {
                    SendMessage(lMessage, true);
                }
            }

            // Remove sent messages
            for (int i = mMessages.Count - 1;i >= 0;i--) {
                IMessage lMessage = mMessages[i];
                if (lMessage.IsSent || lMessage.IsHandled) {
                    mMessages.RemoveAt(i);

                    // If a message is handled (done being processed),
                    // we'll release it for reuse.
                    if (lMessage.IsHandled) {
                        lMessage.Release();
                    }
                }
            }

        }
    }

    /// <summary>
    /// Used by the messenger to hook into the unity update process. This allows us
    /// to delay messages instead of sending them right away. We'll release messages
    /// if a new level is loaded.
    /// </summary>
    public sealed class MessageDispatcherStub:MonoBehaviour {
        /// <summary>
        /// Raised first when the object comes into existance. Called
        /// even if script is not enabled.
        /// </summary>
        void Awake() {
            // Don't destroyed automatically when loading a new scene
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Called after the Awake() and before any update is called.
        /// </summary>
        public IEnumerator Start() {
            // Create the coroutine here so we don't re-create over and over
            WaitForEndOfFrame lWaitForEndOfFrame = new WaitForEndOfFrame();

            // Loop endlessly so we can flag when we're done with the frame
            while (true) {
                yield return lWaitForEndOfFrame;

                // Update the frame index for messages that need to be sent next update.
                // Since the max value of an int is 2,147,483,647, we can run 60FPS
                // for 414 days straight before we hit it. So... we won't worry about extra logic.
                MessageDispatcher.FrameIndex++;
            }
        }

        /// <summary>
        /// Update is called every frame. We pass this to our messenger
        /// </summary>
        void Update() {
            MessageDispatcher.Update();
        }

        /// <summary>
        /// Called when the dispatcher is disabled. We use this to
        /// clean up the event tables everytime a new level loads.
        /// </summary>
        public void OnDisable() {
            MessageDispatcher.ClearMessages();
            MessageDispatcher.ClearListeners();
        }
    }
}
