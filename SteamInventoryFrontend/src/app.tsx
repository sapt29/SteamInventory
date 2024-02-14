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
    if (userIdRef && userIdRef.current) {
      const userId = userIdRef.current.value;
      console.log(userId);
      // if (userId.length != 17) {
      //   return toast.error("User id must contain 17 characters", {
      //     position: "top-right",
      //   });
      // }
      try {

        const x = await fetch(`https://localhost:7155/inventory?userSteamId=${userId}`);
        const a = await x.json();
        console.log(a);

        //const res = await USER_INFO;
        setInfoUser(a);
      } catch (e) {
        const ERRORS = {
          404: "User not found",
          500: "Server error, please try later",
        };
        toast.error("Usuario no encontrado", { position: "top-right" });
        setInfoUser({});
      }
      userIdRef.current.value = "";
      
    }
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
