import ReactApexChart from "react-apexcharts";
const generateChartData = (itemsHistory) => {
  return itemsHistory?.map((data) => {
    return {
      x: new Date(data.datePrice).getTime(),
      y: data.average,
      itemsSold: data.itemsSold,
    };
  });
};
export const PopUp = ({ openPopUp, closePopUp, infoItem }) => {
  const chartData = generateChartData(infoItem?.itemsHistory);
  const handlelosePopUp = (e) => {
    if (e.target.id === "ModelContainer") {
      closePopUp();
    }
  };

  if (openPopUp !== true) return null;
  const options = {
    chart: {
      type: "area",
    },
    sparkline: {
      enabled: true,
    },
    stroke: {
      curve: "smooth",
    },
    fill: {
      type: "gradient",
      gradient: {
        shadeIntensity: 1,
        opacityFrom: 0.7,
        opacityTo: 0.9,
        stops: [0, 90, 100],
      },
    },
    xaxis: {
      type: "datetime",
    },
    dataLabels: {
      enabled: false, // Disable data labels
    },
  };
  return (
    <div
      id="ModelContainer"
      onClick={handlelosePopUp}
      className="fixed inset-0 bg-black flex justify-center items-center bg-opacity-20 backdrop-blur-sm"
    >
      <div className="p-2 bg-white w-10/12 md:w-1/2 lg:1/3 shadow-inner border-e-emerald-600 rounded-lg py-5">
        <div className="w-full p-3 justify-center items-center">
          <h2 className="font-semibold py-3 text-center text-xl">
            {infoItem.itemName}
          </h2>
          <div>
            <div id="chart">
              <ReactApexChart
                type={"area"}
                options={options}
                series={[{ name: "Item Price", data: chartData }]}
              />
            </div>
            <div id="html-dist"></div>
          </div>
        </div>
      </div>
    </div>
  );
};
