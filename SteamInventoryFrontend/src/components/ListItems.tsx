import { PopUp } from "./PopUpInfo";
import { useState } from "preact/hooks";

export const ListItems = ({ items }) => {
  const [itemSelected, setItemSelected] = useState(null);
  const handleRemovePopUp = () => setItemSelected(null);

  return (
    <>
      <div class="container mx-auto p-10">
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-10">
          {items?.map((item) => (
            <div
              class=" p-4  relative cursor-pointer hover:shadow-xl isolate  rounded-xl bg-white/20 shadow-lg ring-1 ring-black/5"
              onClick={() => setItemSelected(item)}
            >
              <img src={item.itemImage} width="100%" height="auto" />
              <h2 class="font-extrabold text-center  dark:text-white">
                {item.itemName}
              </h2>
              <h3 class="font-bold text-center  dark:text-white">
                <strong>Price: </strong>
                {item.priceLatest}
              </h3>
              <h3 class="font-bold text-center  dark:text-white">
                <strong>Quantity: </strong>
                {item.itemQuantity}
              </h3>
              <span
                className={`absolute top-2 right-2 rounded-md p-1  ${
                  item.itemProfitable.includes("-")
                    ? "bg-red-300"
                    : item.itemProfitable.includes("0,00")
                    ? "hidden"
                    : "bg-green-300"
                }`}
              >
                {item.itemProfitable.split(":")[1].trim().replace(".", "")}
              </span>
            </div>
          ))}
        </div>
      </div>
      <PopUp
        openPopUp={!!itemSelected}
        closePopUp={handleRemovePopUp}
        infoItem={itemSelected}
      />
    </>
  );
};
