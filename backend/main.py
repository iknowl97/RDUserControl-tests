from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import List
import random
import string

app = FastAPI()

class User(BaseModel):
    username: str
    displayName: str
    domain: str
    isEnabled: bool
    isRdpEnabled: bool

# In-memory user data (replace with a database in a real app)
users: List[User] = []

@app.get("/users", response_model=List[User])
async def get_users():
    return users

@app.post("/users", response_model=User, status_code=201)
async def create_user(user: User):
    # In a real app, you'd check for duplicates, etc.
    users.append(user)
    return user

@app.put("/users/{username}/enable", response_model=User)
async def enable_user(username: str):
    for user in users:
        if user.username == username:
            user.isEnabled = True
            return user
    raise HTTPException(status_code=404, detail="User not found")

@app.put("/users/{username}/disable", response_model=User)
async def disable_user(username: str):
    for user in users:
        if user.username == username:
            user.isEnabled = False
            return user
    raise HTTPException(status_code=404, detail="User not found")

@app.post("/users/{username}/reset-password", response_model=str)
async def reset_password(username: str):
    # In a real app, you'd hash and securely store the password
    new_password = ''.join(random.choices(string.ascii_letters + string.digits, k=12))
    print(f"Password reset for {username}. New password: {new_password}")  # In real app, email this or similar
    return "Password reset.  Check console for the new password (in a real app, this would be handled securely)."