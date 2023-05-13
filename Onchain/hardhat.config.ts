import {HardhatUserConfig} from "hardhat/config";
import "@nomicfoundation/hardhat-toolbox";


let accounts = {mnemonic: "radar hero man wine confirm try gallery then blue east index because"};

const config: HardhatUserConfig = {
    solidity: "0.8.17",
    defaultNetwork: "chiado",
    networks: {
        hardhat: {},
        gnosis: {
            url: "https://rpc.gnosischain.com",
            accounts,
        },
        chiado: {
            url: "https://rpc.chiadochain.net",
            gasPrice: 1000000000,
            accounts,
        },
    },
    etherscan: {
        customChains: [
            {
                network: "chiado",
                chainId: 10200,
                urls: {
                    apiURL: "https://blockscout.com/gnosis/chiado/api",
                    browserURL: "https://blockscout.com/gnosis/chiado",
                },
            },
            {
                network: "gnosis",
                chainId: 100,
                urls: {
                    apiURL: "https://api.gnosisscan.io/api",
                    browserURL: "https://gnosisscan.io/",
                },
            },
        ],
        apiKey: {
            chiado: "PVFQSY67YV75P99MIJ58QXMXC4E95EWPJM",
            gnosis: "PVFQSY67YV75P99MIJ58QXMXC4E95EWPJM",
        },
    }
};

export default config;