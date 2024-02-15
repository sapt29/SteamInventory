import { Toaster, toast } from "react-hot-toast";
import { useRef, useState } from "preact/hooks";

import { Form } from "@components/Form";
import { ListItems } from "@components/ListItems";
import { USER_INFO } from "./utils/mocks/user";
import { UserInfo } from "@components/UserInfo";

export function App() {
  const userIdRef = useRef<HTMLInputElement>(null);
  const [infoUser, setInfoUser] = useState<any>({});

  const getInfoUser = async (e: any) => {
    e.preventDefault();
    
    if (!userIdRef || !userIdRef.current) return;

    const userId = userIdRef.current.value;

    try {
      const x = await fetch(`https://localhost:7155/inventory?userSteamId=${userId}`);
      if (!x.ok) {
        let errorMessage = "Server error, please try later";
        if (x.status === 400 || x.status === 500) {
          const errorResponse = await x.text();
          errorMessage = errorResponse || errorMessage;
        }
        throw new Error(errorMessage);
      }
      const a = await x.json();
      console.log(a);
      //const res = await USER_INFO;
      setInfoUser(a);
    } catch (e) {
      toast.error(e.message, { position: "top-right" });
      setInfoUser({});
    }
    
    userIdRef.current.value = "";
  }; 

  return (
    <main
      class={`bg-gradient-to-r from-violet-500 to-fuchsia-500 ${
        infoUser.userName ? "h-auto" : "h-screen"
      }`}
    >
      <h1 class="mb-4 text-4xl font-extrabold text-center pt-5 leading-none tracking-tight text-gray-900 md:text-5xl lg:text-6xl dark:text-white">
        Steam Profile
      </h1>      
      <Form userIdRef={userIdRef} searchUserInfo={getInfoUser} />
      <UserInfo {...infoUser} />
      <ListItems {...infoUser} />
      <Toaster />
    </main>
  );
}
