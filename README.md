# untitled-rpg

## **Please Install GitLFS before working for the first time**
   ```sh
   git lfs install
   ```

## **PLEASE RUN THIS COMMAND BEFORE DOING ANY WORK**
   ```sh
   git pull
   ```
   If you run this command and the terminal says that there is a merge conflict, run this command
   ```sh
   git stash
   ```

## Table of Contents
- [Rules](##Rules)
- [Git Cheatsheet](##Git-Cheatsheet)

## **Rules**

### **File Naming Conventions**

#### Each file you create **MUST** have this:
   ```sh
   Name:
   Date of Creation:
   Description:
   ```
at the top of the file.

#### Each new file created must be prefaced with one of these labels:
- AI (Any AI Scripts)
- AN (Animation System)
- CS (Character Systems)
- ES (Enemy System)
- GS (General System) - Catch-all for any systems not mentioned MUST consult eboard before making
- GS (Graphics System)
- MI (Movement/Interaction System)
- PS (Puzzle System)
- UI (User Interface)
- 
The only exception is the Game Manager (there will be only one; please consult with eboard if you want to edit it)

## **Git Cheatsheet**



### **Setup Your Branch**

**Check the current branch:**
   ```sh
   git branch
   ```
**Check what branch you're on**
   ```sh
   git branch new-branch-name
   ```
**Create a new branch**
   ```sh
   git checkout new-branch-name
   ```
**Switch to your branch**
   ```sh
   git checkout -b new-branch-name
   ```
# **PLEASE DO NOT RUN
   ```sh
   git add .
   ```
This will edit all files in the project, and most likely create merge errors for everyone else. Instead:

### Add your files to the main project
   ```sh
   git add "your file names"
   git commit -m "Your comment about what you added"
   git push
   ```
