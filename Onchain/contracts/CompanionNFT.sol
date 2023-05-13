pragma solidity ^0.8.0;

import {ERC721} from "@openzeppelin/contracts/token/ERC721/ERC721.sol";
import "@openzeppelin/contracts/access/Ownable.sol";

contract CompanionNFT is ERC721, Ownable {

    mapping(uint256 => string) public tokenURIs;

    constructor() ERC721("CompanionNFT", "AINFT") {}

    function mint(address collector, uint256 tokenId, string memory tokenURI) public onlyOwner() {
        _safeMint(collector, tokenId);
        _setTokenURI(tokenId, tokenURI);
    }

    function updateMemory(uint256 tokenId, string memory tokenURI) public onlyOwner() {
        _setTokenURI(tokenId, tokenURI);
    }

    function tokenURI(uint256 tokenId) public view virtual override returns (string memory) {
        _requireMinted(tokenId);

        return tokenURIs[tokenId];
    }

    function _setTokenURI(uint256 tokenId, string memory _tokenURI) internal virtual {
        tokenURIs[tokenId] = _tokenURI;
    }
}