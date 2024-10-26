import Image from "next/image";
import DoSomethingButton from "./components/do-something-button";

export default function Home() {
  return (
    <main className="flex min-h-screen flex-col items-center justify-center p-24 bg-gray-950">
      <div className="flex flex-col gap-10 items-center ">
        <h1 className="text-xl font-medium text-white">
          Welcome to the plugin template.
        </h1>
        <DoSomethingButton />
        <div className="flex items-center gap-8">
          <Image src="/pw-logo.png" alt="logo" width={100} height={100} />
          <Image src="/shl-logo.png" alt="logo" width={100} height={100} />
        </div>
      </div>
    </main>
  );
}
