import subprocess
import os
import re

def analyze():
    result = subprocess.run(['git', 'log', '--format=%s'], capture_output=True, text=True)
    commits = result.stdout.lower().split('\n')
    
    fixes = sum(1 for m in commits if any(k in m for k in ['fix', 'fixed', 'bug', 'patch', 'patched']))
    maint = sum(1 for m in commits if any(k in m for k in ['merge', 'pull', 'review']))
    
    total_commits = len([m for m in commits if m.strip()])
    features = total_commits - fixes - maint
    
    relevant_total = fixes + features
    ratio = (fixes / relevant_total * 100) if relevant_total > 0 else 0
    formatted_result = f"Dev Hell Ratio: **{round(ratio, 2)}%**"

    if os.path.exists("README.md"):
        with open("README.md", "r") as f:
            content = f.read()
        
        pattern = r"\[\[DEV_HELL_RATIO\]\]|Dev Hell Ratio: \*\*\d+\.?\d*%\*\*"
        new_content = re.sub(pattern, formatted_result, content)
        
        with open("README.md", "w") as f:
            f.write(new_content)

if __name__ == "__main__":
    analyze()