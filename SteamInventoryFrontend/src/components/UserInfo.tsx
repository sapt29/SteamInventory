export const UserInfo = ({
  userImage,
  userName,
}: {
  userImage: string;
  userName: string;
}) => {
  return (
    <div class="m-auto flex items-center p-10 flex-col">
      {userImage && (
        <img src={userImage} class="object-cover h-32 w-32 rounded-full" />
      )}
      <h1 class="m-auto justify-end text-4xl font-extrabold leading-none tracking-tight dark:text-white md:text-5xl lg:text-6xl">
        {userName}
      </h1>
    </div>
  );
};
