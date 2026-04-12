# Dialogue UI Setup Guide - Complete Instructions

## Prerequisites
- EventSystem in your Scene (automatic, but verify it exists)
- GraphicRaycaster on your Canvas

## Step 1: Canvas Setup
1. Select your **Canvas** in the Hierarchy
2. Verify it has **GraphicRaycaster** component (Add if missing)
3. Set **Render Mode** to "Screen Space - Overlay" or "Screen Space - Camera"
4. Ensure Canvas has a **LayoutGroup** component (optional but recommended)

## Step 2: Create ResponsePanel GameObject
1. Create an empty GameObject under Canvas named "ResponsePanel"
2. Add **RectTransform** component (automatic)
3. Attach **ResponsePanel.cs** script
4. Create a **VerticalLayoutGroup** child:
   - Right-click ResponsePanel → Create Empty
   - Rename to "ButtonContainer"
   - Add component: **VerticalLayoutGroup**
   - Adjust settings:
     - Child Controls Size: X=true, Y=true
     - Child Force Expand: X=false, Y=false
   - In ResponsePanel inspector: Assign ButtonContainer to **Button Container** field

## Step 3: Create ResponseButton Prefab
1. Create a Button UI element:
   - Right-click ButtonContainer → UI (Legacy) → Button - TextMeshPro
   - This creates a Button with automatic setup
2. Rename to "ResponseButton"
3. Verify it has:
   - **RectTransform**
   - **Image** (for highlighting) — Keep it!
   - **Button** component (optional, we handle clicks via code)
   - **CanvasGroup** or **GraphicRaycaster** for raycast blocking
4. Remove child "Text (TMP)" if it exists
5. Create a TextMeshProUGUI child for the response text:
   - Right-click ResponseButton → 3D Object → TextMeshPro - Text
   - Name it "Label"
   - Adjust text settings as needed
6. Add **ResponseButton.cs** script to ResponseButton
7. In Inspector, assign:
   - **Label**: The TextMeshProUGUI child
   - **Background**: The Image component on this button itself
8. Drag ResponseButton into your Prefabs folder to create a prefab
9. **DELETE the instance from the scene** (we'll spawn via code)

## Step 4: Create DialoguePanel GameObject
1. Create an empty GameObject under Canvas named "DialoguePanel"
2. Attach **DialoguePanel.cs** script
3. Create child objects:
   - "SpeakerText" (TextMeshProUGUI)
   - "DialogueText" (TextMeshProUGUI)
   - "ResponsePanel" → Create a ResponsePanel here (or reuse existing)
4. In DialoguePanel Inspector, assign:
   - **Speaker Text**: The SpeakerText child
   - **Dialogue Text**: The DialogueText child
   - **Response Panel**: Your ResponsePanel instance

## Step 5: NPC Setup
1. Select your NPC GameObject
2. Add **DialogueTrigger.cs** script
3. In Inspector, assign:
   - **Dialogue Tree**: Your DialogueTree asset
   - **Dialogue Panel**: The DialoguePanel from your Canvas

## Step 6: Verify EventSystem
1. Select any UI element in Hierarchy
2. In the Debugger, verify you see a parent Canvas with EventSystem
3. Check Project → Stand Alone Input Module (should exist)

## Troubleshooting

### Buttons don't highlight on hover:
- Ensure Image component is on the button itself, not a child
- Check **Background** field in ResponseButton is assigned to the Image
- Verify normal/selected colors are different in ResponsePanel

### Buttons don't click:
- Ensure button has Image component with Source Image set (can be blank)
- Check Raycast Target is enabled on Image
- Verify no panel is blocking raycast (check canvas sorting)
- GraphicRaycaster must be on Canvas

### No buttons appear:
- Verify ButtonContainer is assigned in ResponsePanel
- Check ButtonPrefab is assigned in ResponsePanel
- Ensure ResponsePanel.Show() is being called
- Check button prefab actually has ResponseButton script

## Testing
1. Play the game
2. Move near an NPC
3. Press F to interact (or click if you set it up)
4. Dialogue should appear
5. Hover over response buttons — they should highlight
6. Click response button — dialogue should advance
