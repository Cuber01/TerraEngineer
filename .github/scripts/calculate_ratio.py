import subprocess
import os
import re
import sys  

def get_status(ratio):
    if ratio == 0:
        return "Is this even code?"
    elif ratio < 15:
        return "Life is good!"
    elif ratio < 30:
        return "This is fine ☕"
    elif ratio < 50:
        return "I'm calling an ambulance, but not for me."
    elif ratio < 99:
        return "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"
    else:
        return "Consider removing the repo."

def analyze():
    result = subprocess.run(['git', 'log', '--format=%s'], capture_output=True, text=True)
    commits = [m for m in result.stdout.lower().split('\n') if m.strip()]
    
     total_count = len(commits)
#     if total_count % 25 != 0:
#         print(f"Current commit count is {total_count}. Skipping update (only runs every 25).")
#         sys.exit(0)
    
    fixes = sum(1 for m in commits if any(k in m for k in ['fix', 'fixed', 'bug', 'patch', 'patched']))
    maint = sum(1 for m in commits if any(k in m for k in ['merge', 'pull', 'review', 'action']))
    
    features = total_count - fixes - maint
    
    relevant_total = fixes + features
    ratio = (fixes / relevant_total * 100) if relevant_total > 0 else 0
    status_comment = get_status(ratio)
    
    # The new string we want to put into the README
    formatted_result = f"Dev Hell Ratio: **{round(ratio, 2)}%** — *{status_comment}*"

    if os.path.exists("README.md"):
        with open("README.md", "r") as f:
            content = f.read()
        
        # FIX 3: Updated Regex to match the placeholder OR the previous formatted line
        pattern = r"\[\[DEV_HELL_RATIO\]\]|Dev Hell Ratio: \*\*.*?\*\* — \*.*?\*"
        new_content = re.sub(pattern, formatted_result, content)
        
        with open("README.md", "w") as f:
            f.write(new_content)
        print(f"Successfully updated README at {total_count} commits!")

if __name__ == "__main__":
    analyze()