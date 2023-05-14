import {expect} from "chai";
import {ethers} from "hardhat";

describe("CompanionNFT", function () {
    // async function deploy() {
    //     const [owner, otherAccount] = await ethers.getSigners();
    //
    //     const companionNFT = await ethers.getContractFactory("CompanionNFT");
    //     const contract = await companionNFT.deploy();
    //
    //     return {contract, owner, otherAccount};
    // }

    async function getContract() {
        const [owner] = await ethers.getSigners();
        const MyContract = await ethers.getContractFactory("CompanionNFT");
        const contract = await MyContract.attach(
            "0x1C7Fca9442c07C691d822684d842bDECC017f9Ba"
        );

        return {contract, owner}
    }

    describe("Simple tests", function () {
        it("deploys the contract", async function () {
            const {contract} = await getContract();

            expect(await contract.name()).to.equal("CompanionNFT");
        });

        it("mints NFT with metadata", async function () {
            const {contract, owner} = await getContract();

            const tokenId = 1;
            await contract.mint(owner.address, tokenId, "https://ipfs.io/ipfs/QmeYNQXy1p5HUhFaooDpuSN5PbKfnoKrU8rkcSzwdP28Sw", {
                from: owner.address,
                gasLimit: 1000000
            });

            const tokenURI = await contract.tokenURI(tokenId);
            const data = await (await fetch(tokenURI)).json();

            expect(data.history).to.equal("Some history");
        });
    });


});
