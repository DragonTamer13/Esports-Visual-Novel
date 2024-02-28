// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Text;

namespace Fungus
{
    /// <summary>
    /// Helper class to manage parsing and executing the conversation format.
    /// </summary>
    public class ConversationManager
    {
        const string ConversationTextBodyRegex = @"((?<sayParams>[\w ""><.'-_]*?):)?(?<text>.*)\r*(\n|$)";

        public struct RawConversationItem
        {
            public string[] sayParams;
            public string text;
        }

        protected struct ConversationItem
        {
            public string Text { get; set; }
            public Character Character { get; set; }
            public Sprite Portrait { get; set; }
            public RectTransform Position { get; set; }
            public bool Hide { get; set; }
            public FacingDirection FacingDirection { get; set; }
            public bool Flip { get; set; }
            public bool ClearPrev { get; set; }
            public bool WaitForInput { get; set; }
            public bool FadeDone { get; set; }
            public bool Jump { get; set; }
            public bool Shake { get; set; }
        }

        protected Character[] characters;

        protected bool exitSayWait;
        public bool ClearPrev { get; set; }
        public bool WaitForInput { get; set; }
        public bool FadeDone { get; set; }
        public FloatData WaitForSeconds { get; internal set; }

        public ConversationManager()
        {
            ClearPrev = true;
            FadeDone = true;
            WaitForInput = true;
        }

        /// <summary>
        /// Splits the string passed in by the delimiters passed in.
        /// Quoted sections are not split, and all tokens have whitespace
        /// trimmed from the start and end.
        protected static string[] Split(string stringToSplit)
        {
            var results = new List<string>();

            bool inQuote = false;
            var currentToken = new StringBuilder();
            for (int index = 0; index < stringToSplit.Length; ++index)
            {
                char currentCharacter = stringToSplit[index];
                if (currentCharacter == '"')
                {
                    // When we see a ", we need to decide whether we are
                    // at the start or send of a quoted section...
                    inQuote = !inQuote;
                }
                else if (char.IsWhiteSpace(currentCharacter) && !inQuote)
                {
                    // We've come to the end of a token, so we find the token,
                    // trim it and add it to the collection of results...
                    string result = currentToken.ToString().Trim( new [] { ' ', '\n', '\t', '\"'} );
                    if (result != "") results.Add(result);

                    // We start a new token...
                    currentToken = new StringBuilder();
                }
                else
                {
                    // We've got a 'normal' character, so we add it to
                    // the curent token...
                    currentToken.Append(currentCharacter);
                }
            }

            // We've come to the end of the string, so we add the last token...
            string lastResult = currentToken.ToString().Trim();
            if (lastResult != "") 
            {
                results.Add(lastResult);
            }

            return results.ToArray();
        }

        protected virtual SayDialog GetSayDialog(Character character)
        {
            SayDialog sayDialog = null;
            if (character != null)
            {
                if (character.SetSayDialog != null)
                {
                    sayDialog = character.SetSayDialog;
                }
            }

            if (sayDialog == null)
            {
                sayDialog = SayDialog.GetSayDialog();
            }

            sayDialog.SetActive(true);
            SayDialog.ActiveSayDialog = sayDialog;

            return sayDialog;
        }

        public static void PreParse(string conv, System.Action<RawConversationItem> itemAction)
        {
            //find SimpleScript say strings with portrait options
            //You can test regex matches here: http://regexstorm.net/tester
            var sayRegex = new Regex(ConversationTextBodyRegex);
            MatchCollection sayMatches = sayRegex.Matches(conv);

            for (int i = 0; i < sayMatches.Count; i++)
            {
                string text = sayMatches[i].Groups["text"].Value.Trim();
                string sayParams = sayMatches[i].Groups["sayParams"].Value;

                // As text and SayParams are both optional, an empty string will match the regex.
                // We can ignore any matches where both are empty
                // or if they're Lua style comments
                if ((text.Length == 0 && sayParams.Length == 0) || text.StartsWith("--"))
                {
                    continue;
                }

                string[] separateParams = null;

                if (!string.IsNullOrEmpty(sayParams))
                {
                    separateParams = Split(sayParams);
                }
                else
                {
                    separateParams = new string[0];
                }

                var item = new RawConversationItem() { sayParams = separateParams, text = text };

                itemAction(item);
            }
        }

        public static List<RawConversationItem> PreParse(string conv)
        {
            List<RawConversationItem> retval = new List<RawConversationItem>();

            PreParse(conv, (ia) => { retval.Add(ia); });

            return retval;
        }

        protected virtual List<ConversationItem> Parse(string conv)
        {
            var items = new List<ConversationItem>();

            Character currentCharacter = null;
            PreParse(conv, (ia) =>
            {
                var item = CreateConversationItem(ia.sayParams, ia.text, currentCharacter);

                // Previous speaking character is the default for next conversation item
                currentCharacter = item.Character;
                items.Add(item);
            });

            return items;
        }

        /// <summary>
        /// Using the string of say parameters before the ':',
        /// set the current character, position and portrait if provided.
        /// </summary>
        /// <returns>The conversation item.</returns>
        /// <param name="sayParams">The list of say parameters.</param>
        /// <param name="text">The text for the character to say.</param>
        /// <param name="currentCharacter">The currently speaking character.</param>
        protected virtual ConversationItem CreateConversationItem(string[] sayParams, string text, Character currentCharacter)
        {
            var item = new ConversationItem();
            item.ClearPrev = ClearPrev;
            item.FadeDone = FadeDone;
            item.WaitForInput = WaitForInput;

            // Populate the story text to be written
            item.Text = text;

            if(WaitForSeconds > 0)
            {
                item.Text += "{w=" + WaitForSeconds.ToString() +"}";
            }

            if (sayParams == null || sayParams.Length == 0)
            {
                // Text only, no params - early out.
                return item;
            }

            //TODO this needs a refactor
            for (int i = 0; i < sayParams.Length; i++)
            {
                if (string.Compare(sayParams[i], "clear", true) == 0)
                {
                    item.ClearPrev = true;
                }
                else if (string.Compare(sayParams[i], "noclear", true) == 0)
                {
                    item.ClearPrev = false;
                }
                else if (string.Compare(sayParams[i], "fade", true) == 0)
                {
                    item.FadeDone = true;
                }
                else if (string.Compare(sayParams[i], "nofade", true) == 0)
                {
                    item.FadeDone = false;
                }
                else if (string.Compare(sayParams[i], "wait", true) == 0)
                {
                    item.WaitForInput = true;
                }
                else if (string.Compare(sayParams[i], "nowait", true) == 0)
                {
                    item.WaitForInput = false;
                }
                // By Alex G: Custom param to do small character animations
                else if (string.Compare(sayParams[i], "jump", true) == 0)
                {
                    item.Jump = true;
                }
                else if (string.Compare(sayParams[i], "shake", true) == 0)
                {
                    item.Shake = true;
                }
            }

            // try to find the character param first, since we need to get its portrait
            int characterIndex = -1;
            int characterParamIndexPartial = characterIndex;
            Character partialCharacter = null;
            if (characters == null)
            {
                PopulateCharacterCache();
            }

            for (int i = 0; item.Character == null && i < sayParams.Length; i++)
            {
                for (int j = 0; j < characters.Length; j++)
                {
                    if (characters[j].NameMatch(sayParams[i]))
                    {
                        characterIndex = i;
                        item.Character = characters[j];
                        break;
                    }
                    else if (characters[j].NameStartsWith(sayParams[i]))
                    {
                        characterParamIndexPartial = i;
                        partialCharacter = characters[j];
                    }
                }
            }

            //if still no charcter was found but we found a partial we use it for backcompat but warn user
            if(item.Character == null && partialCharacter != null)
            {
                Debug.LogWarning("Conversation Manager Character Partial Match; found '" + sayParams[characterParamIndexPartial] + 
                    "' and will use " + partialCharacter.NameText + "\n Recommend modifying conversation and characters so they match exactly.");

                characterIndex = characterParamIndexPartial;
                item.Character = partialCharacter;
            }

            // Assume last used character if none is specified now
            if (item.Character == null)
            {
                item.Character = currentCharacter;
            }

            // Check if there's a Hide parameter
            int hideIndex = -1;
            if (item.Character != null)
            {
                for (int i = 0; i < sayParams.Length; i++)
                {
                    if (i != characterIndex &&
                        string.Compare(sayParams[i], "hide", true) == 0 )
                    {
                        hideIndex = i;
                        item.Hide = true;
                        break;
                    }
                }
            }

            int flipIndex = -1;
            if (item.Character != null)
            {
                for (int i = 0; i < sayParams.Length; i++)
                {
                    if (i != characterIndex &&
                        i != hideIndex &&
                        (string.Compare(sayParams[i], ">>>", true) == 0
                         || string.Compare(sayParams[i], "<<<", true) == 0))
                    {
                        if (string.Compare(sayParams[i], ">>>", true) == 0) item.FacingDirection = FacingDirection.Right;
                        if (string.Compare(sayParams[i], "<<<", true) == 0) item.FacingDirection = FacingDirection.Left;
                        flipIndex = i;
                        item.Flip = true;
                        break;
                    }
                }
            }
                
            // Next see if we can find a portrait for this character
            int portraitIndex = -1;
            if (item.Character != null)
            {
                for (int i = 0; i < sayParams.Length; i++)
                {
                    if (item.Portrait == null && 
                        item.Character != null &&
                        i != characterIndex && 
                        i != hideIndex &&
                        i != flipIndex) 
                    {
                        Sprite s = item.Character.GetPortrait(sayParams[i]);
                        if (s != null)
                        {
                            portraitIndex = i;
                            item.Portrait = s;
                            break;
                        }
                    }
                }
            }

            // Next check if there's a position parameter
            Stage stage = Stage.GetActiveStage();
            if (stage != null)
            {
                for (int i = 0; i < sayParams.Length; i++)
                {
                    if (i != characterIndex &&
                        i != portraitIndex &&
                        i != hideIndex)
                    {
                        RectTransform r = stage.GetPosition(sayParams[i]);
                        if (r != null)
                        {
                            item.Position = r;
                            break;
                        }
                    }
                }
            }

            return item;
        }

        #region Public members

        /// <summary>
        /// Caches the character objects in the scene for fast lookup during conversations.
        /// </summary>
        public virtual void PopulateCharacterCache()
        {
            // cache characters for faster lookup
            characters = UnityEngine.Object.FindObjectsOfType<Character>();
        }

        /// <summary>
        /// Parse and execute a conversation string.
        /// </summary>
        public virtual IEnumerator DoConversation(string conv)
        {
            if (string.IsNullOrEmpty(conv))
            {
                yield break;
            }

            var conversationItems = Parse(conv);

            if (conversationItems.Count == 0)
            {
                yield break;
            }

            // Track the current and previous parameter values
            Character currentCharacter = null;
            Sprite currentPortrait = null;
            RectTransform currentPosition = null;
            Character previousCharacter = null;

            // Play the conversation
            for (int i = 0; i < conversationItems.Count; ++i)
            {
                ConversationItem item = conversationItems[i];

                if (item.Character != null)
                {
                    currentCharacter = item.Character;
                }

                currentPortrait = item.Portrait;
                currentPosition = item.Position;

                var sayDialog = GetSayDialog(currentCharacter);

                if (sayDialog == null)
                {
                    // Should never happen
                    yield break;
                }

                if (currentCharacter != null && 
                    currentCharacter != previousCharacter)
                {
                    sayDialog.SetCharacter(currentCharacter);
                }

                //Handle stage changes
                var stage = Stage.GetActiveStage();

                if (currentCharacter != null &&
                    !currentCharacter.State.onScreen &&
                    currentPortrait == null)
                {
                    // No call to show portrait of hidden character
                    // so keep hidden
                    item.Hide = true;
                }

                if (stage != null && currentCharacter != null &&
                    (currentPortrait != currentCharacter.State.portrait ||
                        currentPosition != currentCharacter.State.position ||
                        (currentCharacter.State.display == DisplayType.Hide && !item.Hide)))
                {
                    var portraitOptions = new PortraitOptions(true);
                    portraitOptions.display = item.Hide ? DisplayType.Hide : DisplayType.Show;
                    portraitOptions.character = currentCharacter;
                    portraitOptions.fromPosition = currentCharacter.State.position;
                    portraitOptions.toPosition = currentPosition;
                    portraitOptions.portrait = currentPortrait;

                    //Flip option - Flip the opposite direction the character is currently facing
                    if (item.Flip) portraitOptions.facing = item.FacingDirection;

                    // Do a move tween if the character is already on screen and not yet at the specified position
                    if (currentCharacter.State.onScreen &&
                        currentPosition != currentCharacter.State.position)
                    {
                        portraitOptions.move = true;
                    }

                    if (item.Hide)
                    {
                        stage.Hide(portraitOptions);
                    }
                    else
                    {
                        stage.Show(portraitOptions);
                    }
                }

                if (stage == null &&
                    currentPortrait != null)
                {
                    sayDialog.SetCharacterImage(currentPortrait);
                }

                previousCharacter = currentCharacter;

                if (item.Jump)
                {
                    Vector3[] spline = { currentCharacter.State.holder.gameObject.transform.position,
                        currentCharacter.State.holder.gameObject.transform.position,
                        currentCharacter.State.holder.gameObject.transform.position + new Vector3(0.0f, 10.0f, 0.0f),
                        currentCharacter.State.holder.gameObject.transform.position,
                        currentCharacter.State.holder.gameObject.transform.position};
                    LeanTween.moveSpline(currentCharacter.State.holder.gameObject, spline, 0.5f)
                        .setEase(LeanTweenType.easeSpring);

                    //Debug.Log("Jumping");
                    //Hashtable tweenParams = new Hashtable();
                    //tweenParams.Add("name", "Jump");
                    //tweenParams.Add("amount", new Vector3(0.0f, 1000.0f, 0.0f));
                    //tweenParams.Add("space", Space.Self);
                    //tweenParams.Add("time", 2.0f);
                    //tweenParams.Add("easetype", iTween.EaseType.easeInOutQuad);
                    //tweenParams.Add("looptype", iTween.LoopType.none);
                    //tweenParams.Add("oncomplete", "OniTweenComplete");
                    //tweenParams.Add("oncompletetarget", GameObject.FindObjectOfType<Flowchart>().gameObject);
                    //tweenParams.Add("oncompleteparams", this);
                    //iTween.PunchPosition(currentCharacter.State.holder.gameObject, tweenParams);
                    //LeanTween.moveSpline

                    //Hashtable tweenParams = new Hashtable();
                    //tweenParams.Add("name", "Jump");
                    //tweenParams.Add("amount", new Vector3(0.0f, 600.0f, 0.0f));
                    //tweenParams.Add("space", Space.World);
                    //tweenParams.Add("time", 10.0f);
                    //tweenParams.Add("easetype", iTween.EaseType.easeOutSine);
                    //tweenParams.Add("looptype", iTween.LoopType.none);
                    //tweenParams.Add("oncomplete", "OniTweenComplete");
                    //tweenParams.Add("oncompletetarget", currentCharacter.State.holder.gameObject);
                    //tweenParams.Add("oncompleteparams", this);
                    //iTween.MoveAdd(currentCharacter.State.holder.gameObject, tweenParams);

                    //Transform toTrans = currentCharacter.State.holder.gameObject.transform;
                    //LeanTween.move(currentCharacter.State.holder.gameObject, toTrans, 2.0f)
                    //    .setEase(LeanTweenType.easeShake);
                }

                if (item.Shake)
                {
                    Hashtable tweenParams = new Hashtable();
                    tweenParams.Add("name", "shake");
                    tweenParams.Add("amount", new Vector3(20f, 10f, 0f));
                    tweenParams.Add("time", 0.25f);
                    tweenParams.Add("easetype", iTween.EaseType.easeInOutQuad);
                    tweenParams.Add("looptype", iTween.LoopType.none);
                    tweenParams.Add("isLocal", false);
                    tweenParams.Add("oncomplete", "OniTweenComplete");
                    tweenParams.Add("oncompletetarget", currentCharacter.State.holder.gameObject);
                    tweenParams.Add("oncompleteparams", this);
                    iTween.ShakePosition(currentCharacter.State.holder.gameObject, tweenParams);
                }

                if (!string.IsNullOrEmpty(item.Text)) { 
                    exitSayWait = false;
                    sayDialog.Say(item.Text, item.ClearPrev, item.WaitForInput, item.FadeDone, true, false, null, () => {
                        exitSayWait = true;
                    });

                    while (!exitSayWait)
                    {
                        yield return null;
                    }
                    exitSayWait = false;
                }
            }
        }

        #endregion
    }
}
